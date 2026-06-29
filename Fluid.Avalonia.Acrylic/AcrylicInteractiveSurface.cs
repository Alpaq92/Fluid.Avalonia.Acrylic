using System;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Media;
using global::Avalonia.Rendering;
using global::Avalonia.Threading;

namespace Fluid.Avalonia.Acrylic
{
    /// <summary>
    /// An interactive variant of <see cref="AcrylicSurface"/> that adds press/drag deformation and
    /// an interactive highlight overlay.
    /// </summary>
    public class AcrylicInteractiveSurface : AcrylicSurface, ICustomHitTest
    {
        public static readonly StyledProperty<bool> IsInteractiveProperty =
            AvaloniaProperty.Register<AcrylicInteractiveSurface, bool>(nameof(IsInteractive), true);

        public static readonly StyledProperty<bool> InteractiveHighlightEnabledProperty =
            AvaloniaProperty.Register<AcrylicInteractiveSurface, bool>(nameof(InteractiveHighlightEnabled), true);

        public static readonly StyledProperty<double> InteractiveMaxScaleDipProperty =
            AvaloniaProperty.Register<AcrylicInteractiveSurface, double>(nameof(InteractiveMaxScaleDip), 4.0);

        // When true the press/drag deformation responds to the RIGHT mouse button instead of
        // the left, freeing the left button for other gestures (e.g. dragging the surface).
        public static readonly StyledProperty<bool> DeformOnRightButtonProperty =
            AvaloniaProperty.Register<AcrylicInteractiveSurface, bool>(nameof(DeformOnRightButton), false);

        static AcrylicInteractiveSurface()
        {
            AffectsRender<AcrylicInteractiveSurface>(
                IsInteractiveProperty,
                InteractiveHighlightEnabledProperty,
                InteractiveMaxScaleDipProperty);
        }

        private const double SpringDampingRatio = 0.5;
        private const double SpringStiffness = 300.0;
        private const double SpringProgressThreshold = 0.001;
        private const double InitialDerivative = 0.05;

        private readonly DispatcherTimer _animationTimer;
        private DateTime _lastAnimationTickUtc;

        private int? _activePointerId;
        private TopLevel? _hookedTopLevel;

        private Point _startPosition;
        private SpringDouble _pressProgress = new(SpringStiffness, SpringDampingRatio, SpringProgressThreshold);
        private SpringPoint _position = new(SpringStiffness, SpringDampingRatio, 0.5);

        public AcrylicInteractiveSurface()
        {
            RenderTransformOrigin = RelativePoint.Center;

            _animationTimer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };

            _animationTimer.Tick += (_, _) =>
            {
                DateTime now = DateTime.UtcNow;
                double dt = (now - _lastAnimationTickUtc).TotalSeconds;
                _lastAnimationTickUtc = now;

                // Clamp dt to avoid huge steps on debugger stops / tab switches.
                dt = Math.Max(0.0, Math.Min(0.05, dt));

                bool any = false;
                any |= _pressProgress.Step(dt);
                any |= _activePointerId is null && _position.Step(dt);

                ApplyDeformation();
                InvalidateSubscriberVisual();
                InteractiveOverlay?.InvalidateVisual();

                if (!any)
                    _animationTimer.Stop();
            };

            AddHandler(PointerPressedEvent, OnSelfPointerPressed, RoutingStrategies.Tunnel, true);
        }

        public bool IsInteractive
        {
            get => GetValue(IsInteractiveProperty);
            set => SetValue(IsInteractiveProperty, value);
        }

        public bool InteractiveHighlightEnabled
        {
            get => GetValue(InteractiveHighlightEnabledProperty);
            set => SetValue(InteractiveHighlightEnabledProperty, value);
        }

        public double InteractiveMaxScaleDip
        {
            get => GetValue(InteractiveMaxScaleDipProperty);
            set => SetValue(InteractiveMaxScaleDipProperty, value);
        }

        public bool DeformOnRightButton
        {
            get => GetValue(DeformOnRightButtonProperty);
            set => SetValue(DeformOnRightButtonProperty, value);
        }

        // The glass is drawn via a custom render with no Background, so the control is
        // otherwise invisible to hit-testing (Background="Transparent" doesn't help a
        // custom-rendered control). Claim the whole bounds so press/drag is received
        // anywhere on the surface when interactive.
        public bool HitTest(Point point) => IsInteractive && new Rect(Bounds.Size).Contains(point);

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            UnhookTopLevel();
            _activePointerId = null;
            _animationTimer.Stop();

            // Reset the interaction springs and clear the deform transform. Without this, a surface
            // detached mid-press (tab switch, ItemsControl recycle, virtualization) keeps its pressed
            // spring state, so on re-attach it renders permanently scaled/translated — the timer is
            // stopped and nothing restarts it until the next press.
            _pressProgress.SnapTo(0.0);
            _position.SnapTo(_startPosition);
            RenderTransform = null;
        }

        private void OnSelfPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!IsInteractive)
                return;

            PointerPointProperties props = e.GetCurrentPoint(this).Properties;
            if (!(DeformOnRightButton ? props.IsRightButtonPressed : props.IsLeftButtonPressed))
                return;

            if (_activePointerId is not null)
                return;

            _activePointerId = e.Pointer.Id;
            _startPosition = e.GetPosition(this);
            _position.SnapTo(_startPosition);

            _pressProgress.Target = 1.0;
            _position.Target = _startPosition;

            HookTopLevel();
            ApplyDeformation();
            InvalidateSubscriberVisual();
            InteractiveOverlay?.InvalidateVisual();
            StartAnimation();
        }

        private void OnTopLevelPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_activePointerId is null || e.Pointer.Id != _activePointerId.Value)
                return;

            Point p = e.GetPosition(this);
            _position.SnapTo(p);
            ApplyDeformation();
            InvalidateSubscriberVisual();
            InteractiveOverlay?.InvalidateVisual();
            StartAnimation();
        }

        private void OnTopLevelPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_activePointerId is null || e.Pointer.Id != _activePointerId.Value)
                return;

            EndInteraction();
        }

        private void OnTopLevelPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
        {
            // Treat capture loss as cancellation.
            if (_activePointerId is null)
                return;

            EndInteraction();
        }

        private void EndInteraction()
        {
            _activePointerId = null;
            UnhookTopLevel();

            _pressProgress.Target = 0.0;
            _position.Target = _startPosition;
            ApplyDeformation();
            InvalidateSubscriberVisual();
            InteractiveOverlay?.InvalidateVisual();
            StartAnimation();
        }

        private void HookTopLevel()
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null || ReferenceEquals(topLevel, _hookedTopLevel))
                return;

            UnhookTopLevel();

            _hookedTopLevel = topLevel;
            topLevel.AddHandler(PointerMovedEvent, OnTopLevelPointerMoved, RoutingStrategies.Tunnel, true);
            topLevel.AddHandler(PointerReleasedEvent, OnTopLevelPointerReleased, RoutingStrategies.Tunnel, true);
            topLevel.AddHandler(PointerCaptureLostEvent, OnTopLevelPointerCaptureLost, RoutingStrategies.Tunnel, true);
        }

        private void UnhookTopLevel()
        {
            if (_hookedTopLevel is null)
                return;

            _hookedTopLevel.RemoveHandler(PointerMovedEvent, OnTopLevelPointerMoved);
            _hookedTopLevel.RemoveHandler(PointerReleasedEvent, OnTopLevelPointerReleased);
            _hookedTopLevel.RemoveHandler(PointerCaptureLostEvent, OnTopLevelPointerCaptureLost);
            _hookedTopLevel = null;
        }

        private void StartAnimation()
        {
            if (_animationTimer.IsEnabled)
                return;

            _lastAnimationTickUtc = DateTime.UtcNow;
            _animationTimer.Start();
        }

        private void InvalidateSubscriberVisual()
        {
            AcrylicBackdropProvider.NotifySubscriberOnlyInvalidation(this);
            InvalidateVisual();
        }

        private void ApplyDeformation()
        {
            if (!IsInteractive || Bounds.Width <= 0 || Bounds.Height <= 0)
            {
                RenderTransform = null;
                return;
            }

            double width = Bounds.Width;
            double height = Bounds.Height;

            double progress = Clamp(_pressProgress.Value, 0.0, 1.0);
            double maxScale = Math.Max(0.0, InteractiveMaxScaleDip);

            // Matches LiquidButton.kt layerBlock math (dp converted to px there; DIPs here).
            double scale = Lerp(1.0, 1.0 + maxScale / height, progress);

            Point offset = _position.Value - _startPosition;

            double minDim = Math.Min(width, height);
            double maxDim = Math.Max(width, height);

            double tx = minDim * Math.Tanh(InitialDerivative * offset.X / minDim);
            double ty = minDim * Math.Tanh(InitialDerivative * offset.Y / minDim);

            double maxDragScale = maxScale / height;
            double angle = Math.Atan2(offset.Y, offset.X);

            double aspectX = Math.Min(width / height, 1.0);
            double aspectY = Math.Min(height / width, 1.0);

            double sx =
                scale +
                maxDragScale * Math.Abs(Math.Cos(angle) * offset.X / maxDim) * aspectX;

            double sy =
                scale +
                maxDragScale * Math.Abs(Math.Sin(angle) * offset.Y / maxDim) * aspectY;

            if (Math.Abs(tx) < 0.01 && Math.Abs(ty) < 0.01 && Math.Abs(sx - 1.0) < 0.0005 && Math.Abs(sy - 1.0) < 0.0005)
                RenderTransform = null;
            else
                RenderTransform = new MatrixTransform(Matrix.CreateScale(sx, sy) * Matrix.CreateTranslation(tx, ty));
        }

        internal double GetInteractiveHighlightProgress()
        {
            if (!IsInteractive || !InteractiveHighlightEnabled)
                return 0.0;

            return Clamp(_pressProgress.Value, 0.0, 1.0);
        }

        internal Point GetInteractiveHighlightPosition()
        {
            return _position.Value;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsInteractiveProperty
                || change.Property == InteractiveHighlightEnabledProperty
                || change.Property == InteractiveMaxScaleDipProperty)
            {
                AcrylicBackdropProvider.NotifySubscriberOnlyInvalidation(this);
                InteractiveOverlay?.InvalidateVisual();
            }
        }

        private static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private sealed class SpringDouble
        {
            private readonly double _stiffness;
            private readonly double _dampingRatio;
            private readonly double _threshold;

            public SpringDouble(double stiffness, double dampingRatio, double threshold)
            {
                _stiffness = stiffness;
                _dampingRatio = dampingRatio;
                _threshold = threshold;
            }

            public double Value { get; private set; }
            public double Velocity { get; private set; }
            public double Target { get; set; }

            public void SnapTo(double value)
            {
                Value = value;
                Velocity = 0.0;
            }

            public bool Step(double dt)
            {
                double x = Value;
                double v = Velocity;
                double target = Target;

                double k = _stiffness;
                double c = 2.0 * _dampingRatio * Math.Sqrt(k);

                double a = -k * (x - target) - c * v;
                v += a * dt;
                x += v * dt;

                Value = x;
                Velocity = v;

                bool done = Math.Abs(x - target) <= _threshold && Math.Abs(v) <= _threshold;
                if (done)
                {
                    Value = target;
                    Velocity = 0.0;
                }

                return !done;
            }
        }

        private sealed class SpringPoint
        {
            private readonly SpringDouble _x;
            private readonly SpringDouble _y;

            public SpringPoint(double stiffness, double dampingRatio, double positionThreshold)
            {
                _x = new SpringDouble(stiffness, dampingRatio, positionThreshold);
                _y = new SpringDouble(stiffness, dampingRatio, positionThreshold);
            }

            public Point Value
            {
                get => new(_x.Value, _y.Value);
            }

            public Point Target
            {
                get => new(_x.Target, _y.Target);
                set
                {
                    _x.Target = value.X;
                    _y.Target = value.Y;
                }
            }

            public void SnapTo(Point value)
            {
                _x.SnapTo(value.X);
                _y.SnapTo(value.Y);
            }

            public bool Step(double dt)
            {
                return _x.Step(dt) | _y.Step(dt);
            }
        }
    }
}

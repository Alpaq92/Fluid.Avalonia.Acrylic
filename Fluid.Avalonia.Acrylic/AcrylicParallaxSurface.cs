using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Rendering;

namespace Fluid.Avalonia.Acrylic
{
    /// <summary>
    /// An <see cref="AcrylicSurface"/> variant that shifts the sampled backdrop by a fraction of
    /// the pointer offset from center as the pointer moves over it — a small, self-contained
    /// parallax effect built on the existing <see cref="AcrylicSurface.BackdropOffset"/> sampling
    /// infrastructure (no new capture/render machinery).
    /// </summary>
    public class AcrylicParallaxSurface : AcrylicSurface, ICustomHitTest
    {
        public static readonly StyledProperty<bool> IsParallaxEnabledProperty =
            AvaloniaProperty.Register<AcrylicParallaxSurface, bool>(nameof(IsParallaxEnabled), true);

        /// <summary>
        /// Maximum backdrop shift (DIPs) applied when the pointer reaches the edge of the surface.
        /// </summary>
        public static readonly StyledProperty<double> ParallaxStrengthProperty =
            AvaloniaProperty.Register<AcrylicParallaxSurface, double>(nameof(ParallaxStrength), 24.0);

        public AcrylicParallaxSurface()
        {
            PointerMoved += OnParallaxPointerMoved;
            PointerExited += OnParallaxPointerExited;
        }

        public bool IsParallaxEnabled
        {
            get => GetValue(IsParallaxEnabledProperty);
            set => SetValue(IsParallaxEnabledProperty, value);
        }

        public double ParallaxStrength
        {
            get => GetValue(ParallaxStrengthProperty);
            set => SetValue(ParallaxStrengthProperty, value);
        }

        // AcrylicSurface draws via a custom render with no Background, so it is otherwise
        // invisible to hit-testing (mirrors AcrylicInteractiveSurface.HitTest) — claim the whole
        // bounds so pointer-move parallax responds anywhere on the surface, not just over content.
        public bool HitTest(Point point) => IsParallaxEnabled && new Rect(Bounds.Size).Contains(point);

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            BackdropOffset = default;
        }

        private void OnParallaxPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!IsParallaxEnabled || Bounds.Width <= 0 || Bounds.Height <= 0)
                return;

            Point p = e.GetPosition(this);
            double halfWidth = Bounds.Width / 2.0;
            double halfHeight = Bounds.Height / 2.0;

            double nx = Clamp((p.X - halfWidth) / halfWidth, -1.0, 1.0);
            double ny = Clamp((p.Y - halfHeight) / halfHeight, -1.0, 1.0);

            BackdropOffset = new Vector(nx * ParallaxStrength, ny * ParallaxStrength);
        }

        private void OnParallaxPointerExited(object? sender, PointerEventArgs e)
        {
            BackdropOffset = default;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}

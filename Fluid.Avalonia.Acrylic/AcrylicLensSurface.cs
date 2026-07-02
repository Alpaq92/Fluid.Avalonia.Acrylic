using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Rendering;

namespace Fluid.Avalonia.Acrylic
{
    /// <summary>
    /// An <see cref="AcrylicSurface"/> variant that shows a small circular magnifying-glass lens
    /// centered on the pointer while it hovers over the surface — a plain zoom, not a refraction/
    /// bend effect. It affects only a small, pointer-following area and only ever feeds the Lens
    /// pass (behind the ContentPresenter), the same guarantee <see cref="AcrylicParallaxSurface"/>
    /// gives its BackdropOffset shift.
    /// </summary>
    public class AcrylicLensSurface : AcrylicSurface, ICustomHitTest
    {
        public static readonly StyledProperty<bool> IsLensEnabledProperty =
            AvaloniaProperty.Register<AcrylicLensSurface, bool>(nameof(IsLensEnabled), true);

        /// <summary>Radius (DIPs) of the hover lens.</summary>
        public static readonly StyledProperty<double> LensRadiusProperty =
            AvaloniaProperty.Register<AcrylicLensSurface, double>(nameof(LensRadius), 60.0);

        /// <summary>Magnification factor inside the lens (1 = no zoom).</summary>
        public static readonly StyledProperty<double> LensZoomProperty =
            AvaloniaProperty.Register<AcrylicLensSurface, double>(nameof(LensZoom), 1.6);

        // Live pointer-driven state, kept local to this subclass (not on the shared AcrylicSurface
        // base) — mirrors how AcrylicParallaxSurface only ever touches the pre-existing
        // BackdropOffset property rather than adding base-class fields of its own.
        private Point _hoverPosition;
        private double _hoverRadius;
        private double _hoverZoom = 1.0;

        public AcrylicLensSurface()
        {
            PointerMoved += OnLensPointerMoved;
            PointerExited += OnLensPointerExited;
        }

        public bool IsLensEnabled
        {
            get => GetValue(IsLensEnabledProperty);
            set => SetValue(IsLensEnabledProperty, value);
        }

        public double LensRadius
        {
            get => GetValue(LensRadiusProperty);
            set => SetValue(LensRadiusProperty, value);
        }

        public double LensZoom
        {
            get => GetValue(LensZoomProperty);
            set => SetValue(LensZoomProperty, value);
        }

        // AcrylicSurface draws via a custom render with no Background, so it is otherwise
        // invisible to hit-testing (mirrors AcrylicParallaxSurface.HitTest) — claim the whole
        // bounds so the lens responds anywhere on the surface, not just over content.
        public bool HitTest(Point point) => IsLensEnabled && new Rect(Bounds.Size).Contains(point);

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _hoverRadius = 0;
        }

        internal override void ApplyHoverLens(ref AcrylicDrawParameters parameters)
        {
            parameters.LensPosition = _hoverPosition;
            parameters.LensRadius = _hoverRadius;
            parameters.LensZoom = _hoverZoom;
        }

        private void OnLensPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!IsLensEnabled || Bounds.Width <= 0 || Bounds.Height <= 0)
            {
                _hoverRadius = 0;
                InvalidateVisual();
                return;
            }

            _hoverPosition = e.GetPosition(this);
            _hoverRadius = LensRadius;
            _hoverZoom = LensZoom;
            InvalidateVisual();
        }

        private void OnLensPointerExited(object? sender, PointerEventArgs e)
        {
            _hoverRadius = 0;
            InvalidateVisual();
        }
    }
}

using global::Avalonia;
using global::Avalonia.Media;

namespace Fluid.Avalonia.Acrylic
{
    internal enum AcrylicDrawPass
    {
        Lens,
        InteractiveHighlight,
        Highlight,
        Reveal
    }

    internal struct AcrylicDrawParameters
    {
        public CornerRadius CornerRadius { get; set; }

        public double BackdropZoom { get; set; }
        public Vector BackdropOffset { get; set; }

        public double RefractionHeight { get; set; }
        public double RefractionAmount { get; set; }
        public bool DepthEffect { get; set; }
        public bool ChromaticAberration { get; set; }

        public Point LensPosition { get; set; }
        public double LensRadius { get; set; }
        public double LensZoom { get; set; }
        public double LensDarken { get; set; }
        public double LensRingIntensity { get; set; }
        public double LensSharpness { get; set; }

        public double BlurRadius { get; set; }
        public double Vibrancy { get; set; }
        public double Brightness { get; set; }
        public double Contrast { get; set; }
        public double ExposureEv { get; set; }
        public double GammaPower { get; set; }
        public double BackdropOpacity { get; set; }

        public Color TintColor { get; set; }
        public Color SurfaceColor { get; set; }

        public bool ProgressiveBlurEnabled { get; set; }
        public double ProgressiveBlurStart { get; set; }
        public double ProgressiveBlurEnd { get; set; }
        public Color ProgressiveTintColor { get; set; }
        public double ProgressiveTintIntensity { get; set; }

        public bool HighlightEnabled { get; set; }
        public double HighlightWidth { get; set; }
        public double HighlightBlurRadius { get; set; }
        public double HighlightOpacity { get; set; }
        public double HighlightAngleDegrees { get; set; }
        public double HighlightFalloff { get; set; }

        public double InteractiveProgress { get; set; }
        public Point InteractivePosition { get; set; }

        public double RevealProgress { get; set; }
        public Point RevealPosition { get; set; }
        public double RevealRadius { get; set; }
        public double RevealWidth { get; set; }
        public Color RevealColor { get; set; }

        public bool ShadowEnabled { get; set; }
        public double ShadowRadius { get; set; }
        public Vector ShadowOffset { get; set; }
        public Color ShadowColor { get; set; }
        public double ShadowOpacity { get; set; }

        public bool InnerShadowEnabled { get; set; }
        public double InnerShadowRadius { get; set; }
        public Vector InnerShadowOffset { get; set; }
        public Color InnerShadowColor { get; set; }
        public double InnerShadowOpacity { get; set; }
    }
}

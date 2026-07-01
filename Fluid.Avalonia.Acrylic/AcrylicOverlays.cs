using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Media;

namespace Fluid.Avalonia.Acrylic
{
    internal sealed class AcrylicInteractiveOverlay : Control
    {
        public override void Render(DrawingContext context)
        {
            if (AcrylicBackdropProvider.IsCapturing)
                return;

            if (TemplatedParent is not AcrylicInteractiveSurface surface)
                return;

            if (!surface.InteractiveHighlightEnabled)
                return;

            double progress = surface.GetInteractiveHighlightProgress();
            if (progress <= 0.001)
                return;

            Rect bounds = new(0, 0, Bounds.Width, Bounds.Height);
            AcrylicDrawParameters parameters = surface.CreateDrawParameters();
            parameters.InteractiveProgress = progress;
            parameters.InteractivePosition = surface.GetInteractiveHighlightPosition();

            context.Custom(new AcrylicDrawOperation(bounds, parameters, null, AcrylicDrawPass.InteractiveHighlight));
        }
    }

    internal sealed class AcrylicFrontOverlay : Control
    {
        public override void Render(DrawingContext context)
        {
            if (AcrylicBackdropProvider.IsCapturing)
                return;

            if (TemplatedParent is not AcrylicSurface surface)
                return;

            Rect bounds = new(0, 0, Bounds.Width, Bounds.Height);
            AcrylicDrawParameters parameters = surface.CreateDrawParameters();

            if (surface.HighlightEnabled)
                context.Custom(new AcrylicDrawOperation(bounds, parameters, null, AcrylicDrawPass.Highlight));

            if (surface.InnerShadowEnabled && parameters.InnerShadowOpacity > 0.001 && parameters.InnerShadowColor.A > 0)
                context.Custom(new AcrylicInnerShadowDrawOperation(bounds, parameters));

            if (surface.RevealBorderEnabled && parameters.RevealProgress > 0.001)
                context.Custom(new AcrylicDrawOperation(bounds, parameters, null, AcrylicDrawPass.Reveal));
        }
    }
}

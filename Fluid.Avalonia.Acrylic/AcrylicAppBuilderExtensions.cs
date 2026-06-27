using global::Avalonia;
using global::Avalonia.Rendering.Composition;

namespace Fluid.Avalonia.Acrylic
{
    public static class AcrylicAppBuilderExtensions
    {
        public static AppBuilder UseAcrylicPerformanceDefaults(
            this AppBuilder builder,
            long skiaMaxGpuResourceSizeBytes = 256L * 1024L * 1024L,
            int maxDirtyRects = 8)
        {
            return builder
                .With(new CompositionOptions
                {
                    UseRegionDirtyRectClipping = true,
                    MaxDirtyRects = maxDirtyRects
                })
                .With(new SkiaOptions
                {
                    MaxGpuResourceSizeBytes = skiaMaxGpuResourceSizeBytes
                });
        }
    }
}

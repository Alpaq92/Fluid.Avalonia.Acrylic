using global::Avalonia;
using global::Avalonia.Rendering.Composition;

namespace Fluid.Avalonia.Acrylic
{
    public static class AcrylicAppBuilderExtensions
    {
        /// <summary>
        /// Applies opinionated Composition/Skia options tuned for the acrylic backdrop (region
        /// dirty-rect clipping + a GPU resource cap). NOTE: this <b>replaces</b> any
        /// <see cref="CompositionOptions"/>/<see cref="SkiaOptions"/> set earlier in the builder
        /// chain — call it before your own <c>.With(...)</c> overrides if you need to customise them.
        /// </summary>
        public static AppBuilder UseAcrylicPerformanceDefaults(
            this AppBuilder builder,
            long skiaMaxGpuResourceSizeBytes = 256L * 1024L * 1024L,
            int maxDirtyRects = 8)
        {
            if (skiaMaxGpuResourceSizeBytes <= 0)
                throw new global::System.ArgumentOutOfRangeException(nameof(skiaMaxGpuResourceSizeBytes));
            if (maxDirtyRects <= 0)
                throw new global::System.ArgumentOutOfRangeException(nameof(maxDirtyRects));

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

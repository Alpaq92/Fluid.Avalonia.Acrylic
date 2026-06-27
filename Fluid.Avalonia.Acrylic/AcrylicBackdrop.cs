using System;
using global::Avalonia;
using global::Avalonia.VisualTree;

namespace Fluid.Avalonia.Acrylic
{
    public sealed class AcrylicBackdrop
    {
        private AcrylicBackdrop()
        {
        }

        public static readonly AttachedProperty<bool> IsExcludedFromCaptureProperty =
            AvaloniaProperty.RegisterAttached<AcrylicBackdrop, Visual, bool>(
                "IsExcludedFromCapture",
                false);

        public static bool GetIsExcludedFromCapture(Visual visual)
        {
            if (visual is null)
                throw new ArgumentNullException(nameof(visual));

            return visual.GetValue(IsExcludedFromCaptureProperty);
        }

        public static void SetIsExcludedFromCapture(Visual visual, bool value)
        {
            if (visual is null)
                throw new ArgumentNullException(nameof(visual));

            visual.SetValue(IsExcludedFromCaptureProperty, value);
        }
    }
}

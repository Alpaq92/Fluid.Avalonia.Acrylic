using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Fluid.Avalonia.Acrylic;

namespace AcrylicDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Live-tune the Playground card from the side controls.
        BlurSlider.PropertyChanged += (_, e) =>
        {
            if (e.Property == RangeBase.ValueProperty) PlayCard.BlurRadius = BlurSlider.Value;
        };
        VibSlider.PropertyChanged += (_, e) =>
        {
            if (e.Property == RangeBase.ValueProperty) PlayCard.Vibrancy = VibSlider.Value;
        };
        BrightSlider.PropertyChanged += (_, e) =>
        {
            if (e.Property == RangeBase.ValueProperty) PlayCard.Brightness = BrightSlider.Value;
        };
        HighlightCheck.PropertyChanged += (_, e) =>
        {
            if (e.Property == ToggleButton.IsCheckedProperty) PlayCard.HighlightEnabled = HighlightCheck.IsChecked ?? true;
        };
        ShadowCheck.PropertyChanged += (_, e) =>
        {
            if (e.Property == ToggleButton.IsCheckedProperty) PlayCard.ShadowEnabled = ShadowCheck.IsChecked ?? true;
        };
    }

    public void OnSwatch(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string hex })
            PlayCard.TintColor = Color.Parse(hex);
    }
}

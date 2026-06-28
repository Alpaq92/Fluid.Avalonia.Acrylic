using Avalonia;
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

        WireInteractiveDrag();
    }

    // Interactive tab: left-drag moves the card around the canvas; right-drag deforms it
    // (AcrylicInteractiveSurface handles the right-button deformation itself).
    private void WireInteractiveDrag()
    {
        var card = InteractiveCard;
        var canvas = InteractiveCanvas;
        bool moving = false;
        Point start = default;
        double startLeft = 0, startTop = 0;

        card.PointerPressed += (_, e) =>
        {
            if (!e.GetCurrentPoint(card).Properties.IsLeftButtonPressed)
                return;
            moving = true;
            start = e.GetPosition(canvas);
            startLeft = Canvas.GetLeft(card);
            startTop = Canvas.GetTop(card);
            e.Pointer.Capture(card);
            e.Handled = true;
        };
        card.PointerMoved += (_, e) =>
        {
            if (!moving)
                return;
            var pos = e.GetPosition(canvas);
            Canvas.SetLeft(card, startLeft + (pos.X - start.X));
            Canvas.SetTop(card, startTop + (pos.Y - start.Y));
        };
        card.PointerReleased += (_, e) =>
        {
            if (!moving)
                return;
            moving = false;
            e.Pointer.Capture(null);
        };
    }

    public void OnSwatch(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string hex })
            PlayCard.TintColor = Color.Parse(hex);
    }
}

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace AcrylicDemo;

public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        // Desktop: a classic window. Browser (WASM) and other single-view hosts: the
        // shell view is the top level itself.
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            singleView.MainView = new MainView();

        base.OnFrameworkInitializationCompleted();
    }
}

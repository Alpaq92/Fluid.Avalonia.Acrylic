using System;
using System.Linq;
using Avalonia;

namespace AcrylicDemo;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // `dotnet run -- --shoot` renders each tab headlessly to ./shots (used to
        // verify the build); with no args it launches the normal desktop window.
        if (args.Contains("--shoot"))
        {
            Shoot.Run();
            return;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

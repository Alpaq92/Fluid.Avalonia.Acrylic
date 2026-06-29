using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AcrylicDemo;

// Headless render of every tab — used to verify the build renders correctly and
// to regenerate the README screenshots. Run with: dotnet run -- --shoot
internal static class Shoot
{
    public static void Run()
    {
        AppBuilder.Configure<App>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            .WithInterFont()
            .SetupWithoutStarting();

        var outDir = Path.Combine(AppContext.BaseDirectory, "shots");
        Directory.CreateDirectory(outDir);

        var window = new MainWindow { Width = 1180, Height = 760 };
        window.Show();

        // MainWindow hosts MainView (a separate XAML name scope), so reach the
        // TabControl through the visual tree rather than by name.
        Pump();
        var tabs = window.GetVisualDescendants().OfType<TabControl>().First();
        string[] names = { "gallery", "playground", "interactive", "in-app" };

        for (int i = 0; i < tabs.ItemCount; i++)
        {
            tabs.SelectedIndex = i;
            Pump();

            var frame = window.CaptureRenderedFrame();
            var path = Path.Combine(outDir, $"tab-{i}-{names[i]}.png");
            frame?.Save(path);
            Console.WriteLine($"shot {path} ({(frame is null ? "NULL" : frame.PixelSize.ToString())})");
        }

        window.Close();
        Console.WriteLine("shots -> " + outDir);
    }

    private static void Pump()
    {
        for (int t = 0; t < 20; t++)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        }
    }
}

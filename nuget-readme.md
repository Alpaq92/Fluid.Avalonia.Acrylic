# Fluid.Avalonia.Acrylic

A cross-platform, software-rendered **acrylic / frosted-glass backdrop** for [Avalonia](https://avaloniaui.net) — one SkiaSharp shader, every platform. It renders entirely through Avalonia's Skia backend, so it works on Windows, macOS, Linux and the browser (WebAssembly) with **no OS backdrop support** (Mica / vibrancy / KWin blur) required.

Companion to [Fluid.Avalonia](https://github.com/Alpaq92/Fluid.Avalonia), but dependency-free and usable in any Avalonia app.

## Get started

```
dotnet add package Fluid.Avalonia.Acrylic
```

Requires Avalonia **12+** with the Skia renderer (the default on desktop and browser).

```xml
xmlns:acrylic="using:Fluid.Avalonia.Acrylic"
```

```xml
<!-- A frosted-glass card over whatever is behind it -->
<acrylic:AcrylicSurface CornerRadius="28" BlurRadius="8" Vibrancy="1.5" TintColor="#3B82F6">
    <!-- your content -->
</acrylic:AcrylicSurface>
```

- **`AcrylicSurface`** — a `ContentControl` that frosts whatever is behind it and clips to `CornerRadius` (`BlurRadius`, `Vibrancy`, `Brightness`, `TintColor`, `RefractionHeight` / `RefractionAmount`, `HighlightEnabled`, `ShadowEnabled`).
- **`AcrylicInteractiveSurface`** — adds press/drag deformation and an interactive highlight.

For best performance, opt into the tuned defaults at startup:

```csharp
AppBuilder.Configure<App>().UseAcrylicPerformanceDefaults();
```

A live browser demo, screenshots and full documentation are on the [project page](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic).

## License

[MIT](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/blob/main/LICENSE) — a repackaged fork of [LiquidGlassAvaloniaUI](https://github.com/KaranocaVe/LiquidGlassAvaloniaUI) by KaranocaVe (MIT). All rendering credit belongs to the upstream project.

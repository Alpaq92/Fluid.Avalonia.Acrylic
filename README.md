<div align="center">

<img src="https://raw.githubusercontent.com/Alpaq92/Fluid.Avalonia.Acrylic/main/icon.png" width="128" height="128" alt="Fluid.Avalonia.Acrylic icon" />

# Fluid.Avalonia.Acrylic

A cross-platform, software-rendered **acrylic / frosted-glass backdrop** for [AvaloniaUI](https://avaloniaui.net).

[![NuGet](https://img.shields.io/nuget/v/Fluid.Avalonia.Acrylic.svg)](https://www.nuget.org/packages/Fluid.Avalonia.Acrylic)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

</div>

A SkiaSharp `SKRuntimeEffect` shader pipeline — *vibrancy → blur → rounded-rect lens refraction → tint → edge highlight* — rendered entirely through Avalonia's Skia backend. Because it's software-rendered, it looks the same on **Windows, macOS, Linux and the browser (WASM)**, with **no OS backdrop support required** (unlike Mica / vibrancy / KWin blur).

Companion package to **[Fluid.Avalonia](https://github.com/Alpaq92/Fluid.Avalonia)** — though it has no dependency on it and works in any Avalonia app.

## Get started

```
dotnet add package Fluid.Avalonia.Acrylic
```

Requires Avalonia **12+** with the Skia renderer (`Avalonia.Skia`, the default on desktop and browser).

```xml
xmlns:acrylic="using:Fluid.Avalonia.Acrylic"
```

```xml
<!-- A frosted-glass card over whatever is behind it -->
<acrylic:AcrylicSurface CornerRadius="28"
                        BlurRadius="8"
                        Vibrancy="1.5"
                        TintColor="#3B82F6">
    <!-- your content -->
</acrylic:AcrylicSurface>
```

- **`AcrylicSurface`** — a `ContentControl` that draws the glass pipeline behind its child and clips to `CornerRadius`. Key properties: `BlurRadius`, `Vibrancy`, `Brightness`, `TintColor`, `RefractionHeight` / `RefractionAmount`, `HighlightEnabled`, `ShadowEnabled`.
- **`AcrylicInteractiveSurface`** — adds press/drag deformation and an interactive highlight.

For best performance, opt into the tuned compositor/Skia defaults at startup:

```csharp
AppBuilder.Configure<App>()
    .UseAcrylicPerformanceDefaults();
```

## Credits

This is a renamed, repackaged fork of **[LiquidGlassAvaloniaUI](https://github.com/KaranocaVe/LiquidGlassAvaloniaUI)** by **KaranocaVe** (MIT) — the name was changed to "Acrylic" as a more platform-neutral term. All rendering credit belongs to the upstream project; see [LICENSE](LICENSE) for the preserved copyright.

## License

[MIT](LICENSE) — © 2025 KaranocaVe (original), © 2026 Alpaq92 (fork).

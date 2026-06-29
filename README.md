# Fluid.Avalonia.Acrylic

A cross-platform, software-rendered **acrylic / frosted-glass backdrop** for [AvaloniaUI](https://avaloniaui.net).

[![NuGet](https://img.shields.io/nuget/v/Fluid.Avalonia.Acrylic.svg)](https://www.nuget.org/packages/Fluid.Avalonia.Acrylic)
[![Live demo](https://img.shields.io/badge/demo-live-success)](https://alpaq92.github.io/Fluid.Avalonia.Acrylic/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/blob/main/LICENSE)

One shader, every platform.

Companion package to **[Fluid.Avalonia](https://github.com/Alpaq92/Fluid.Avalonia)** — though it has no dependency on it and works in any Avalonia app.

## Live demo

Try it right in your browser — the demo runs on Avalonia's **WebAssembly** head, deployed to **GitHub Pages**:

> **Live demo →** **<https://alpaq92.github.io/Fluid.Avalonia.Acrylic/>**

## Screenshots

<p align="center">
  <img src="https://raw.githubusercontent.com/Alpaq92/Fluid.Avalonia.Acrylic/main/docs/acrylic-hero.png" width="880" alt="A frosted-glass AcrylicSurface card over a vibrant backdrop" />
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/Alpaq92/Fluid.Avalonia.Acrylic/main/docs/acrylic-showcase.png" width="880" alt="Three AcrylicSurface cards with different tints frosting a colorful backdrop" />
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/Alpaq92/Fluid.Avalonia.Acrylic/main/docs/acrylic-cool.png" width="820" alt="A tunable AcrylicSurface card with heavier blur over a cool backdrop" />
</p>

> Real `AcrylicSurface` output — identical on Windows, macOS, Linux and the browser (WASM).

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
- **`AcrylicInteractiveSurface`** — adds press/drag deformation and an interactive highlight. Set `DeformOnRightButton` to arm the deformation with the right mouse button, leaving the left button free for your own gestures (e.g. dragging the surface).

For best performance, opt into the tuned compositor/Skia defaults at startup:

```csharp
AppBuilder.Configure<App>()
    .UseAcrylicPerformanceDefaults();
```

## Credits

This is a renamed, repacked and updated fork of **[LiquidGlassAvaloniaUI](https://github.com/KaranocaVe/LiquidGlassAvaloniaUI)** by **KaranocaVe** (MIT) with confirmed support for Avalonia 12 — the name was changed to "Acrylic" as a more platform-neutral term. All rendering credit belongs to the upstream project; see [LICENSE](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/blob/main/LICENSE) for the preserved copyright.

## License

[MIT](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/blob/main/LICENSE) — © 2025 KaranocaVe (original), © 2026 Alpaq92 (fork).

# Fluid.Avalonia.Acrylic

A cross-platform, software-rendered **acrylic / frosted-glass backdrop** for [AvaloniaUI](https://avaloniaui.net).

[![NuGet](https://img.shields.io/nuget/v/Fluid.Avalonia.Acrylic.svg?label=NuGet&color=blue)](https://www.nuget.org/packages/Fluid.Avalonia.Acrylic)
[![Downloads](https://img.shields.io/nuget/dt/Fluid.Avalonia.Acrylic.svg?label=Downloads&color=blue)](https://www.nuget.org/packages/Fluid.Avalonia.Acrylic)
[![CI](https://img.shields.io/github/actions/workflow/status/Alpaq92/Fluid.Avalonia.Acrylic/ci.yml?branch=main&label=CI)](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/actions/workflows/ci.yml)
[![Release](https://img.shields.io/github/actions/workflow/status/Alpaq92/Fluid.Avalonia.Acrylic/release.yml?branch=main&label=Release)](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/actions/workflows/release.yml)
[![Live demo](https://img.shields.io/badge/demo-live-success)](https://alpaq92.github.io/Fluid.Avalonia.Acrylic/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/blob/main/LICENSE)

One shader, every platform.

Companion package to **[Fluid.Avalonia](https://github.com/Alpaq92/Fluid.Avalonia)** — though it has no dependency on it and works in any Avalonia app.

## Live demo

Try it right in your browser — the demo runs on Avalonia's **WebAssembly** head, deployed to **GitHub Pages**:

> **Live demo →** **<https://alpaq92.github.io/Fluid.Avalonia.Acrylic/>**

## Screenshots

<p align="center">
  <img src="https://raw.githubusercontent.com/Alpaq92/Fluid.Avalonia.Acrylic/main/assets/acrylic-hero.png" width="880" alt="A frosted-glass AcrylicSurface card over a vibrant backdrop" />
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/Alpaq92/Fluid.Avalonia.Acrylic/main/assets/acrylic-showcase.png" width="880" alt="Three AcrylicSurface cards with different tints frosting a colorful backdrop" />
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/Alpaq92/Fluid.Avalonia.Acrylic/main/assets/acrylic-cool.png" width="820" alt="A tunable AcrylicSurface card with heavier blur over a cool backdrop" />
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

## How this compares

**Vs. native window blur** (Avalonia's `TransparencyLevelHint="AcrylicBlur"/"Mica"` +
`ExperimentalAcrylicBorder`, or FluentAvalonia/SukiUI/Semi.Avalonia, which consume it) — those
blur the *desktop* via OS composition: Windows/macOS only, no browser, and
[flaky on Windows 11](https://github.com/AvaloniaUI/Avalonia/issues/6465). `AcrylicSurface`
blurs the *app's own rendered content* in a Skia shader, so it's identical on Windows, macOS,
Linux and WASM — that's the gap this library fills, not a replacement for native blur.

**Vs. [DaisyGlass](https://github.com/tobitege/Flowery.NET)** — the one other Avalonia library
doing in-content glass the same way (sample-and-blur, not OS composition). DaisyGlass ships a
fast, good-enough *default look* (no real backdrop blur unless you opt in — its default mode
is static gradients, zero backdrop sampling) inside a much broader DaisyUI-style component
kit. `AcrylicSurface` does one thing: a live, shader-accurate pipeline — true lens refraction,
adaptive luminance, progressive blur, spring-physics press/drag, and a pointer-tracked reveal
highlight — that samples the real backdrop continuously and by default, with no toggle that
silently turns the glass fake. Choose DaisyGlass if you want a glass *look* alongside a full
design system; choose `AcrylicSurface` if the glass itself needs to be correct, live, and
interactive.

## Documentation

- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) — the rendering pipeline, module map, and
  code provenance.
- [`docs/CONTRIBUTION.md`](docs/CONTRIBUTION.md) — building, conventions, and how to submit a
  change.

## Credits

This started as a renamed, repacked fork of **[LiquidGlassAvaloniaUI](https://github.com/KaranocaVe/LiquidGlassAvaloniaUI)** by **KaranocaVe** (MIT) with confirmed support for Avalonia 12 — the name was changed to "Acrylic" as a more platform-neutral term. It's no longer a simple fork: the core glass pipeline (including `AcrylicInteractiveSurface`'s press/drag deformation) is still inherited, but `RevealBorderEnabled`'s pointer-tracked glow and `AcrylicParallaxSurface` were built natively with no upstream equivalent — see [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md#code-provenance) for the detail. All credit for the inherited pipeline belongs to the upstream project; see [LICENSE](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/blob/main/LICENSE) for the preserved copyright.

## License

[MIT](https://github.com/Alpaq92/Fluid.Avalonia.Acrylic/blob/main/LICENSE) — © 2025 KaranocaVe (original), © 2026 Alpaq92 (fork).

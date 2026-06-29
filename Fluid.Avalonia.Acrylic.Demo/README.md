# Fluid.Avalonia.Acrylic.Demo

An Avalonia demo for **[Fluid.Avalonia.Acrylic](../README.md)** (`AcrylicSurface`), running on both
desktop and the browser (WebAssembly) — handy for trying the effect and grabbing screenshots.

> **Live demo →** <https://alpaq92.github.io/Fluid.Avalonia.Acrylic/>

## Projects

- **`Fluid.Avalonia.Acrylic.Demo`** — shared library: the `App`, `MainWindow`, `MainView`, and tab
  content. References the sibling `Fluid.Avalonia.Acrylic` library project, so it runs without the
  NuGet package being published.
- **`Fluid.Avalonia.Acrylic.Demo.Desktop`** — the Windows/macOS/Linux entry point.
- **`Fluid.Avalonia.Acrylic.Demo.Browser`** — the WebAssembly head deployed to GitHub Pages.

## Run

Desktop:

```
dotnet run --project Fluid.Avalonia.Acrylic.Demo.Desktop
```

Browser (WebAssembly) — needs the `wasm-tools` workload (`dotnet workload install wasm-tools`):

```
dotnet run --project Fluid.Avalonia.Acrylic.Demo.Browser
```

## Tabs

- **Gallery** — three frosted cards (blue / grape / teal) over a vibrant backdrop.
- **Playground** — one card plus live sliders (blur, vibrancy, brightness), highlight/shadow toggles, and tint swatches.
- **Interactive** — `AcrylicInteractiveSurface`: left-drag moves the card, right-drag deforms it (squish & spring).
- **In an app** — frosted sidebar + content panel in a realistic layout.

## Headless render

```
dotnet run --project Fluid.Avalonia.Acrylic.Demo.Desktop -- --shoot
```

Renders every tab to `bin/…/shots/` — a build check and a way to regenerate the package README images.

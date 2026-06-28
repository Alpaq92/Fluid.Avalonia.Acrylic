# Fluid.Avalonia.Acrylic.Demo

A small Avalonia desktop app showing **[Fluid.Avalonia.Acrylic](../README.md)** (`AcrylicSurface`) in several scenes — handy for trying the effect and grabbing screenshots.

## Run

From the repository root:

```
dotnet run --project Fluid.Avalonia.Acrylic.Demo
```

It references the sibling `Fluid.Avalonia.Acrylic` library project, so it runs without the NuGet package being published.

## Tabs

- **Gallery** — three frosted cards (blue / grape / teal) over a vibrant backdrop.
- **Playground** — one card plus live sliders (blur, vibrancy, brightness), highlight/shadow toggles, and tint swatches.
- **Interactive** — `AcrylicInteractiveSurface`: **left-drag** moves the card, **right-drag** deforms it (squish & spring, via `DeformOnRightButton`).
- **In an app** — frosted sidebar + content panel in a realistic layout.

## Headless render

```
dotnet run --project Fluid.Avalonia.Acrylic.Demo -- --shoot
```

Renders every tab to `bin/…/shots/` — a build check and a way to regenerate the package README images.

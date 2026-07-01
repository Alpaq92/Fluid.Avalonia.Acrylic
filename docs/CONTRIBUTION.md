# Contributing

See [`ARCHITECTURE.md`](ARCHITECTURE.md) first for how the pieces fit together — this doc is
about the mechanics of making and submitting a change.

## Building

```
dotnet restore Fluid.Avalonia.Acrylic.slnx
dotnet build Fluid.Avalonia.Acrylic.slnx --configuration Release
```

That's what CI runs (`.github/workflows/ci.yml`), and it builds all four projects: the library
(`net8.0`), the shared demo UI, and both heads (`Fluid.Avalonia.Acrylic.Demo.Desktop`,
`Fluid.Avalonia.Acrylic.Demo.Browser` on `net10.0-browser`). Building just the browser head
requires the WASM workload once: `dotnet workload install wasm-tools`. Avalonia's version is
pinned once in `Directory.Build.props` (`$(AvaloniaVersion)`) — bump it there, not per-project.

There is currently no automated test project. Verify a change by building clean and exercising
the demo:

- **Desktop:** `dotnet run --project Fluid.Avalonia.Acrylic.Demo.Desktop`.
- **Browser:** `dotnet run --project Fluid.Avalonia.Acrylic.Demo.Browser`, then open the printed
  `http://localhost:5080/` URL. Note that some headless/sandboxed browser automation lacks
  WebGL entirely (`canvas.getContext('webgl')` returns `null`), which the Skia/WASM renderer
  needs — if a screenshot tool renders a blank canvas, check for that before assuming the
  library is broken.
- **Headless, no GUI/browser needed:** `Avalonia.Headless` + `Avalonia.Skia`
  (`AppBuilder.Configure<App>().UseSkia().UseHeadless(new AvaloniaHeadlessPlatformOptions {
  UseHeadlessDrawing = false })`) renders real frames to `RenderTargetBitmap` via
  `window.CaptureRenderedFrame()`, and `Avalonia.Headless`'s `MouseMove`/`MouseDown` extensions
  can drive synthetic pointer input. This is the fastest way to confirm a rendering or
  pointer-interaction change actually produces the pixels you expect, independent of any GUI.

## Conventions

- **Every `using` is `global::`-qualified** (`using global::Avalonia;`, not `using Avalonia;`)
  — consistent across the whole library, keep it that way in new files.
- **No unnecessary abstractions.** A bug fix doesn't need a surrounding refactor; three similar
  lines beat a premature helper. This codebase stays direct on purpose — see any existing file
  for the level of indirection to match.
- **Comments explain WHY, not WHAT.** Good names carry the "what." A comment earns its place
  only when it captures a non-obvious constraint, a workaround, or a reason something is *not*
  done the simpler way — see e.g. the lease/dispose comments in `AcrylicBackdropSnapshot` or
  the `TransformToVisual` direction note in `AcrylicRevealBroadcaster`.
- **Shader assets aren't a wildcard include.** Adding a new `.sksl` file means adding both a
  `<None Remove="Assets\Shaders\Foo.sksl" />` and `<AvaloniaResource Include="..." />` line to
  `Fluid.Avalonia.Acrylic.csproj`, and loading it in `AcrylicDrawOperation.LoadShaders()`.
- **Commit messages are [Conventional Commits](https://www.conventionalcommits.org/)**
  (`feat:`, `fix:`, `chore:`, `docs:`, with an optional scope like `fix(backdrop): ...`).
  `release.yml` runs `release-please`, which parses these to generate `CHANGELOG.md` and bump
  the package version automatically — an inaccurate prefix produces an inaccurate changelog
  entry.

## Automation you'll see on a PR

- **`ci.yml`** — the build above, on every PR and push to `main`.
- **`codeql.yml`** — static analysis.
- **`auto-merge-trusted.yml` / `auto-merge-approved.yml` / `dependabot-auto-merge.yml`** —
  auto-merge for trusted/approved PRs and Dependabot version bumps once checks pass.
- **`pages.yml`** — deploys the Browser demo head to GitHub Pages from `main`.
- **`release.yml`** — `release-please`; merging to `main` with Conventional Commit messages is
  what eventually cuts a release and updates `CHANGELOG.md`. Don't hand-edit the changelog.

## Licensing

The library is a fork of the MIT-licensed
[LiquidGlassAvaloniaUI](https://github.com/KaranocaVe/LiquidGlassAvaloniaUI) — see
[`ARCHITECTURE.md`](ARCHITECTURE.md#code-provenance) for how much of the current tree traces
back to that fork versus original work. Contributions are MIT, same as the rest of the repo.

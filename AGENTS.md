# AGENTS.md

CNC (ЧПУ) machining calculator — Blazor WebAssembly PWA. All code, comments, XML docs, UI strings, and commit messages are in **Russian**; match that style.

## Stack & requirements

- **.NET 10** (`net10.0`) for every project. No `global.json` — needs the .NET 10 SDK.
- Windows + Visual Studio 2026 (VS 18) solution, but plain `dotnet` CLI works for build/test/publish.

## Projects

- `Sunduk.PWA/` — the actual app. Blazor WASM PWA, `AssemblyName` is `Sunduk` (not `Sunduk.PWA`). UI is MudBlazor **9.8.0** + CodeBeam MudBlazor.Extensions, Blazored.LocalStorage, BlazorDownloadFile, Blazor.Text.Editor, NCalcSync (calculator expressions).
- `Sunduk.Geometry/` — shared geometry library (tool-nose compensation, contour elements `Arc/Line/Point`). Referenced by PWA + both test projects.
- `Sunduk.Tests/` — xUnit tests for the PWA CAM G-code generation (`Operation.RoughTurning` etc.).
- `Sunduk.Geometry.Tests/` — xUnit numeric tests for `ToolTipCompensation`.
- `Sunduk.WebApi/`, `Sunduk.DT.Api/`, `Sunduk.Feedback/` — small deployable backends (feedback e-mail sender, DT template lister, server-side Blazor feedback page). `Sunduk.Feedback` uses MudBlazor **6.0.5** — do not "fix" the version mismatch with the PWA blindly.

## Commands

```sh
dotnet build Sunduk.sln
dotnet test                                   # runs both xUnit projects
dotnet run --project Sunduk.PWA               # local WASM dev server
dotnet publish Sunduk.PWA/Sunduk.PWA.csproj -c Release --output release
```

Run a single test project: `dotnet test Sunduk.Geometry.Tests/Sunduk.Geometry.Tests.csproj`.

## Deployment & git

- Two branches: `master` (prod → sunduk.one) and `test` (WIP → test.sunduk.one).
- Deploy is GitHub Actions → GitHub Pages (`gh-pages` / `gh-test-pages`), triggered on push. `api-deploy.yml` publishes `Sunduk.WebApi` to a separate `sunduk-api` repo.
- `Sunduk.WebApi` reads SMTP credentials (`FeedbackFrom`/`FeedbackTo`/`FeedbackPass`) from user-secrets (set in CI from repo secrets). Never hardcode or commit them.
- Gotcha: `main.yml` (prod) still sets up .NET **9** while `main-test.yml` uses .NET **10** and the code targets `net10.0` — prod deploy may be stale.

## Persistence gotcha

`Sunduk.PWA/Infrastructure/State/MachineRegistry.cs` persists machines/tools in LocalStorage and applies **one-time migrations on load** (`MigrateXxx` methods). Any schema change to `Machine`/`Tool` (or its JSON shape) needs a matching migration or existing users silently lose/misload data. Migrations merge with seeds by `Id` rather than overwriting user edits.

## Tests

- xUnit (`[Fact]`), numeric asserts use 4-decimal precision (`Assert.Equal(expected, actual, 4)`).
- Some expected values in `ToolTipCompensationTests` are marked `CAD` — unverified and must be confirmed in CAD before trusting.
- `Sunduk.Tests` does structural G-code checks (cycle emitted, profile block present), not exact coordinate snapshots.

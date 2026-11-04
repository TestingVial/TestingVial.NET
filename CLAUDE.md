# TestingVial.NET

The `TestingVial.NET` NuGet package: attributes that tag tests with the vial (business feature) and test type they cover.

Part of the TestingVial workspace: cross-repo rules, issue tracking and the propagation checklist are in `../CLAUDE.md`
(present when this repo is cloned inside the `TestingVial/workspace` checkout). Every change needs an issue on
<https://github.com/orgs/TestingVial/projects/1> referenced from its PR.

## Build and test

```bash
dotnet build TestingVial.NET.slnx
dotnet test TestingVial.NET.slnx
```

- `TestingVial.NET/` — the library, multi-targeting net8.0, net9.0, net10.0, no dependencies.
- `TestingVial.Examples/` — NUnit examples on net8.0 and net10.0, the LTS frameworks (project reference to the library);
  doubles as the only test project. Running it needs only the .NET 8 and 10 runtimes.
- CI: `.github/workflows/ci.yml` builds, tests and packs on every pull request and on push to `main`.

## Public API (all in namespace `TestingVial.NET`)

`IVial` (`Name`), abstract `VialAttribute` (`VialName`, `TestType`, `Description`; ctor `(name, testType)`), abstract
`TestingVialAttribute<T>` (`: base(new T().Name, testType)`), and `UnitTestVial`/`UnitTestVial<T>`,
`IntegrationTestVial`/`IntegrationTestVial<T>`, `EndToEndTestVial`/`EndToEndTestVial<T>` (test types `"Unit"`,
`"Integration"`, `"E2E"`).

**Pipette.NET depends on this API through Roslyn**, by metadata name and constructor shape, not by compiling against it. Treat
any rename or signature change of these types, their properties, or their constructor parameters as a breaking change that must
be propagated to Pipette (`Analysis/*`, `docs/testingvial-api.md`, fixtures). Test type strings (`"Unit"`, `"Integration"`, ...)
end up in `.vial` files and must map to a value the VialSchemas schema allows.

## Packaging

Versions live in `TestingVial.NET/TestingVial.NET.csproj` (`PackageVersion`, `AssemblyVersion`, `FileVersion`). They are currently
inconsistent (1.0.1 / 0.0.1 / 1.0.0.1); keep them aligned when releasing. The package ships the root `README.md` (the user
documentation — update it with any API change) and `icon.png`.

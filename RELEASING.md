# Releasing TestingVial.NET

## Versions

`<VersionPrefix>` in `TestingVial.NET/TestingVial.NET.csproj` is the single source of truth:

| Version | Value for `<VersionPrefix>1.1.0</VersionPrefix>` |
|---|---|
| Package version | `1.1.0` (`1.1.0-beta1` when packed with `-p:VersionSuffix=beta1`) |
| File version | `1.1.0.0` |
| Assembly version | `1.0.0.0`: fixed per major version, so code compiled against one 1.x binds to any other 1.x |

Follow [Semantic Versioning](https://semver.org/): a new attribute or analyzer rule is a minor release; renaming or removing a
type, property or constructor parameter is a major release (and breaks [Pipette.NET](https://github.com/TestingVial/Pipette.NET),
which reads them by name).

## Steps

1. On a branch, set `<VersionPrefix>` and move the rules of `TestingVial.NET.Analyzers/AnalyzerReleases.Unshipped.md` to a
   `## Release <version>` section in `AnalyzerReleases.Shipped.md`. Open a PR and merge it once CI is green.
2. Tag the merge commit on `main` and push the tag:

   ```bash
   git tag v1.1.0
   git push origin v1.1.0
   ```

3. Pack from the tag and check the package (the analyzer is in `analyzers/dotnet/cs`, the README and icon are at the root):

   ```bash
   dotnet pack TestingVial.NET/TestingVial.NET.csproj -c Release -o artifacts
   ```

4. Publish it to nuget.org with an API key that has push rights on `TestingVial.NET`:

   ```bash
   dotnet nuget push artifacts/TestingVial.NET.1.1.0.nupkg --api-key <key> --source https://api.nuget.org/v3/index.json
   ```

5. Create a GitHub release for the tag with the notable changes.
6. Bump the `TestingVial.NET` package references in Pipette.NET (`tests/Pipette.Core.Tests`, `tests/Fixtures`) and update its
   `docs/testingvial-api.md`.

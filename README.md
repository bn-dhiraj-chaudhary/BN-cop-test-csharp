# BN-cop-test-csharp

Minimal .NET/C# console project seeded with intentional findings (hardcoded credentials, SQL
injection, OS command injection, path traversal, weak/broken cryptographic hash) so a
Coverity on Polaris (COP) scan and a later Black Duck Polaris scan of the same code both
produce comparable issues/merge keys.

## Usage

1. Push this folder as its own repo to GitHub.
2. In COP, create a project pointing at this repo and run a scan using `COP.yml`.
   Record the resulting issues and merge keys.
3. Migrate the project to Polaris via the migration tool.
4. Run a Polaris scan of the same repo/commit (the pipeline reuses `COP.yml` as the
   analyze config — see `.github/workflows/pipeline.yml`).
5. Compare issues and merge keys between the two scans.

## Required GitHub secrets

- `COP_SERVER_URL`, `COP_ACCESS_TOKEN` — used when the pipeline is dispatched with `target: cop`
- `POLARIS_SERVER_URL`, `POLARIS_ACCESS_TOKEN` — used when dispatched with `target: polaris`

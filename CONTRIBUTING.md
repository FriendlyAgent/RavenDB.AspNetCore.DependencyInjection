# Contributing to RavenDB.AspNetCore.DependencyInjection

Thank you for your interest in contributing! Here are some guidelines to help you get started.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later
- [Docker](https://www.docker.com/get-started) (for running RavenDB)

## Setting Up the Development Environment

1. Clone the repository:

```shell
git clone https://github.com/FriendlyAgent/RavenDB.AspNetCore.DependencyInjection.git
cd RavenDB.AspNetCore.DependencyInjection
```

2. Start RavenDB:

```shell
docker compose up -d
```

3. Build the solution:

```shell
dotnet build
```

4. Run the tests:

```shell
dotnet test
```

Tests require a running RavenDB instance at `http://localhost:8080`. Each test creates an isolated database that is automatically cleaned up.

## Branch Strategy

- `master` -- stable release branch
- `develop` -- active development branch

Please submit pull requests against the **`develop`** branch.

## Submitting Changes

1. Open an issue first to discuss larger changes
2. Fork the repository and create a feature branch from `develop`
3. Follow existing code style and patterns
4. Add or update tests for your changes
5. Ensure all tests pass (`dotnet test`)
6. Ensure the solution builds without warnings (`dotnet build`)
7. Submit a pull request against `develop`

## Reporting Issues

If you find a bug or have a feature request, please [open an issue](https://github.com/FriendlyAgent/RavenDB.AspNetCore.DependencyInjection/issues).

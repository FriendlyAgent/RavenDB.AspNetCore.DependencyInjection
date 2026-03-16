# RavenDB.AspNetCore.DependencyInjection

Dependency injection package for using RavenDB with ASP.NET Core. Manages `IDocumentStore` lifecycle and provides scoped `IDocumentSession` / `IAsyncDocumentSession` via DI.

[![NuGet Version](https://img.shields.io/nuget/v/RavenDB.AspNetCore.DependencyInjection.svg?style=flat)](https://www.nuget.org/packages/RavenDB.AspNetCore.DependencyInjection/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RavenDB.AspNetCore.DependencyInjection.svg?style=flat)](https://www.nuget.org/packages/RavenDB.AspNetCore.DependencyInjection/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Two Approaches](#two-approaches)
- [Configuration](#configuration)
- [Session Registration](#session-registration)
- [Certificate Authentication](#certificate-authentication)
- [Keyed Services](#keyed-services)
- [Session Factory](#session-factory)
- [Advanced Session Configuration](#advanced-session-configuration)
- [Dynamic Cluster Management](#dynamic-cluster-management)
- [Extensibility](#extensibility)
- [Samples](#samples)
- [Development Setup](#development-setup)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Two approaches** -- `RavenStore` for single-cluster apps, `RavenManager` for multi-cluster
- **Fully modular** -- every component (manager, session factory, sessions) can be replaced with your own implementation
- Multiple cluster support with one `IDocumentStore` per cluster for optimal resource usage
- High availability via multiple URLs per cluster with automatic failover
- Scoped `IDocumentSession` and `IAsyncDocumentSession` with automatic lifecycle management
- Keyed services support for .NET 8+ (`[FromKeyedServices]`)
- `IRavenSessionFactory` for creating multiple sessions with different configurations within one scope
- Flexible certificate authentication (file, certificate store, PEM, Base64, or direct object)
- Per-cluster or global document conventions
- Dynamic cluster management at runtime (add/remove clusters)
- Configuration via code or `appsettings.json`
- Supports **.NET 8**, **.NET 9**, and **.NET 10**
- Compatible with **RavenDB 6.x** and **7.x**

## Installation

```shell
dotnet add package RavenDB.AspNetCore.DependencyInjection
```

## Quick Start

The fastest way to get started -- register a single RavenDB connection and inject sessions:

```csharp
builder.Services
    .AddRavenStore(options =>
    {
        options.Urls = new[] { "https://localhost:8080" };
        options.DefaultDatabase = "MyDatabase";
    })
    .AddScopedAsyncSession();
```

Then inject `IAsyncDocumentSession` anywhere:

```csharp
public class HomeController : Controller
{
    private readonly IAsyncDocumentSession _session;

    public HomeController(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _session.Query<User>().ToListAsync();
        return View(users);
    }
}
```

That's it -- the store is registered as a singleton, sessions are scoped per request, and disposal is handled automatically.

## Two Approaches

| | RavenStore | RavenManager |
|---|---|---|
| **Extension** | `AddRavenStore()` | `AddRavenManager()` |
| **Clusters** | Single | Multiple |
| **Use case** | One database connection | Multi-cluster, multi-tenant |
| **Store registration** | `IDocumentStore` singleton | One `IDocumentStore` per cluster via `IRavenManager` |
| **Session factory** | Yes | Yes |
| **Keyed services** | Yes (.NET 8+) | Yes (.NET 8+) |

**Use `RavenStore` when** you only need one cluster connection. It's simpler and registers `IDocumentStore` directly in DI.

**Use `RavenManager` when** you need multiple cluster connections (e.g., production + reporting, multi-tenant with separate clusters). It maintains exactly one store per cluster and lets you switch between them.

## Configuration

### RavenStore (Single Cluster)

**Programmatic:**

```csharp
builder.Services
    .AddRavenStore(options =>
    {
        options.Urls = new[] { "https://node1.example.com", "https://node2.example.com" };
        options.DefaultDatabase = "MyDatabase";
    })
    .AddScopedAsyncSession();
```

**From appsettings.json:**

```json
{
    "Raven": {
        "Urls": ["https://node1.example.com", "https://node2.example.com"],
        "DefaultDatabase": "MyDatabase"
    }
}
```

```csharp
builder.Services
    .AddRavenStore(builder.Configuration.GetSection("Raven"))
    .AddScopedAsyncSession();
```

### RavenManager (Multiple Clusters)

**Programmatic:**

```csharp
builder.Services
    .AddRavenManager(options =>
    {
        options.DefaultCluster = "Production";

        options.AddCluster("Production", new RavenStoreOptions
        {
            Urls = new[] { "https://prod-node1.example.com", "https://prod-node2.example.com" },
            DefaultDatabase = "ProductionDB"
        });

        options.AddCluster("Reporting", new RavenStoreOptions
        {
            Urls = new[] { "https://reporting.example.com" },
            DefaultDatabase = "ReportingDB"
        });
    })
    .AddScopedAsyncSession();
```

**From appsettings.json:**

```json
{
    "Raven": {
        "DefaultCluster": "Production",
        "Clusters": {
            "Production": {
                "Urls": ["https://prod-node1.example.com", "https://prod-node2.example.com"],
                "DefaultDatabase": "ProductionDB"
            },
            "Reporting": {
                "Urls": ["https://reporting.example.com"],
                "DefaultDatabase": "ReportingDB"
            }
        }
    }
}
```

```csharp
builder.Services
    .AddRavenManager(builder.Configuration.GetSection("Raven"))
    .AddScopedAsyncSession();
```

Inject `IAsyncDocumentSession` for the default cluster, or use `IRavenManager` to access specific clusters:

```csharp
public class HomeController : Controller
{
    private readonly IAsyncDocumentSession _session;
    private readonly IRavenManager _ravenManager;

    public HomeController(IAsyncDocumentSession session, IRavenManager ravenManager)
    {
        _session = session;
        _ravenManager = ravenManager;
    }

    public async Task<IActionResult> Index()
    {
        // Default cluster session (injected via DI)
        var users = await _session.Query<User>().ToListAsync();

        // Specific cluster session (via manager)
        var reportingSession = _ravenManager.GetAsyncSession("Reporting");
        var reports = await reportingSession.Query<Report>().ToListAsync();

        return View();
    }
}
```

### Options Reference

**RavenStoreOptions** (used for both `AddRavenStore` and as cluster options in `RavenManager`):

| Property | Description |
|----------|-------------|
| `Urls` | Array of cluster node URLs for high availability and automatic failover |
| `DefaultDatabase` | Database name (required unless overridden per session) |
| `CertificateOptions` | Certificate configuration (see [Certificate Authentication](#certificate-authentication)) |
| `Conventions` | [Document conventions](https://ravendb.net/docs/csharp/client-api/configuration/conventions/what-are-conventions) for this cluster |

**RavenManagerOptions** (additional properties):

| Property | Description |
|----------|-------------|
| `DefaultCluster` | Cluster key used when no specific cluster is specified |
| `DefaultConventions` | Default conventions applied to all clusters unless overridden |
| `DefaultCertificateOptions` | Default certificate applied to all clusters unless overridden |
| `Clusters` | Dictionary of cluster keys to `RavenStoreOptions` |
| `AddCluster(key, options)` | Add a cluster configuration |

## Session Registration

Both builders provide the same session registration methods:

**AddScopedAsyncSession()** -- Registers `IAsyncDocumentSession` with a request-scoped lifetime.

**AddScopedSession()** -- Registers `IDocumentSession` with a request-scoped lifetime.

### RavenStore overloads

```csharp
.AddScopedAsyncSession()                        // Default database
.AddScopedAsyncSession("OtherDatabase")          // Specific database
.AddScopedAsyncSession(new SessionOptions { })   // With SessionOptions

.AddScopedSession()                              // Synchronous equivalents
.AddScopedSession("OtherDatabase")
.AddScopedSession(new SessionOptions { })
```

### RavenManager overloads

```csharp
.AddScopedAsyncSession()                                    // Default cluster + database
.AddScopedAsyncSession("Reporting")                          // Specific cluster
.AddScopedAsyncSession("Reporting", "AnalyticsDB")           // Cluster + database
.AddScopedAsyncSession(new SessionOptions { })               // Default cluster with SessionOptions
.AddScopedAsyncSession("Reporting", new SessionOptions { })  // Cluster + SessionOptions

.AddScopedSession()                                          // Synchronous equivalents
.AddScopedSession("Reporting")
.AddScopedSession("Reporting", "AnalyticsDB")
.AddScopedSession(new SessionOptions { })
.AddScopedSession("Reporting", new SessionOptions { })
```

## Certificate Authentication

The `RavenCertificateOptions` class supports five certificate loading methods, checked in priority order:

| Method | Properties | Use case |
|--------|-----------|----------|
| **Direct object** | `Certificate` | Pre-loaded certificate in code |
| **File path** | `CertificatePath`, `CertificatePassword` | PFX/P12 file on disk |
| **Certificate store** | `CertificateThumbprint`, `CertificateStoreName`, `CertificateStoreLocation` | Windows/macOS certificate store |
| **PEM files** | `CertificatePemPath`, `CertificateKeyPath`, `CertificateKeyPassword` | Separate PEM cert + key files |
| **Base64** | `CertificateBase64`, `CertificatePassword` | Environment variables or config |

### Examples

**PFX file:**

```csharp
builder.Services.AddRavenStore(options =>
{
    options.Urls = new[] { "https://a.example.ravendb.cloud" };
    options.DefaultDatabase = "MyDatabase";
    options.CertificateOptions = new RavenCertificateOptions
    {
        CertificatePath = "certs/ravendb.pfx",
        CertificatePassword = "password"
    };
});
```

**Certificate store (thumbprint):**

```csharp
options.CertificateOptions = new RavenCertificateOptions
{
    CertificateThumbprint = "ABC123DEF456...",
    CertificateStoreName = StoreName.My,
    CertificateStoreLocation = StoreLocation.CurrentUser
};
```

**From appsettings.json (file path):**

```json
{
    "Raven": {
        "Urls": ["https://a.example.ravendb.cloud"],
        "DefaultDatabase": "MyDatabase",
        "CertificateOptions": {
            "CertificatePath": "certs/ravendb.pfx",
            "CertificatePassword": "password"
        }
    }
}
```

**Default certificate for all clusters:**

```csharp
builder.Services.AddRavenManager(options =>
{
    // Applied to all clusters that don't specify their own CertificateOptions
    options.DefaultCertificateOptions = new RavenCertificateOptions
    {
        CertificatePath = "certs/ravendb.pfx",
        CertificatePassword = "password"
    };

    options.DefaultCluster = "Production";
    options.AddCluster("Production", new RavenStoreOptions
    {
        Urls = new[] { "https://a.example.ravendb.cloud" },
        DefaultDatabase = "ProductionDB"
    });
});
```

## Keyed Services

On .NET 8+, you can register multiple sessions with different keys using `[FromKeyedServices]`:

### RavenStore

```csharp
builder.Services
    .AddRavenStore(options => { ... })
    .AddKeyedAsyncSession("readonly", new SessionOptions { NoTracking = true })
    .AddKeyedAsyncSession("default")
    .AddKeyedSession("sync");
```

### RavenManager

```csharp
builder.Services
    .AddRavenManager(options => { ... })
    .AddKeyedAsyncSession("production", "Production")
    .AddKeyedAsyncSession("reporting", "Reporting")
    .AddKeyedAsyncSession("readonly", "Production", new SessionOptions { NoTracking = true });
```

Then inject using the key:

```csharp
public class ReportController : Controller
{
    public ReportController(
        [FromKeyedServices("production")] IAsyncDocumentSession productionSession,
        [FromKeyedServices("reporting")] IAsyncDocumentSession reportingSession)
    {
        // ...
    }
}
```

## Session Factory

For scenarios where you need multiple sessions with different configurations within the same scope, register the `IRavenSessionFactory`:

```csharp
builder.Services
    .AddRavenManager(options => { ... })
    .AddSessionFactory();
```

Then inject and create sessions on demand:

```csharp
public class BatchProcessor
{
    private readonly IRavenSessionFactory _factory;

    public BatchProcessor(IRavenSessionFactory factory)
    {
        _factory = factory;
    }

    public async Task ProcessAsync()
    {
        // Create multiple sessions with different configurations
        var readSession = _factory.CreateAsyncSession("Production",
            new SessionOptions { NoTracking = true });

        var writeSession = _factory.CreateAsyncSession("Production");

        var reportingSession = _factory.CreateAsyncSession("Reporting");
    }
}
```

> **Note:** `IRavenSessionFactory` also works with `AddRavenStore` for single-cluster setups. In that case, use the overloads without a cluster key.

## Advanced Session Configuration

Both `RavenStore` and `RavenManager` support RavenDB's native `SessionOptions` for advanced scenarios:

```csharp
// At registration
builder.Services.AddRavenStore(options => { ... })
    .AddScopedAsyncSession(new SessionOptions
    {
        NoTracking = true,
        NoCaching = true,
        TransactionMode = TransactionMode.ClusterWide,
        MaxNumberOfRequestsPerSession = 50,
        Database = "SpecificDatabase"
    });

// At runtime via IRavenManager
var session = ravenManager.GetAsyncSession("Production", new SessionOptions
{
    NoTracking = true,
    NoCaching = true
});
```

### Common SessionOptions

| Option | Description |
|--------|-------------|
| `NoTracking` | Disable entity change tracking for read-only scenarios (better performance) |
| `NoCaching` | Disable session-level cache to always get fresh data from server |
| `TransactionMode` | `SingleNode` (faster) or `ClusterWide` (safer) |
| `MaxNumberOfRequestsPerSession` | Prevent N+1 query problems (default: 30) |
| `UseOptimisticConcurrency` | Enable optimistic concurrency for all operations |
| `Database` | Override the default database for this session |

See [RavenDB SessionOptions documentation](https://ravendb.net/docs/csharp/client-api/session/opening-a-session) for all options.

### Custom Document Conventions

```csharp
builder.Services.AddRavenManager(options =>
{
    // Global conventions for all clusters
    options.DefaultConventions = new DocumentConventions
    {
        IdentityPartsSeparator = "-",
        MaxNumberOfRequestsPerSession = 50,
        UseOptimisticConcurrency = true
    };

    options.DefaultCluster = "Primary";
    options.AddCluster("Primary", new RavenStoreOptions
    {
        Urls = new[] { "https://ravendb.example.com" },
        DefaultDatabase = "MyDB",
        // Override conventions for this specific cluster
        Conventions = new DocumentConventions
        {
            IdentityPartsSeparator = "_",
            MaxNumberOfRequestsPerSession = 100
        }
    });
});
```

## Dynamic Cluster Management

`IRavenManager` supports adding and removing clusters at runtime:

```csharp
public class TenantService
{
    private readonly IRavenManager _ravenManager;

    public TenantService(IRavenManager ravenManager)
    {
        _ravenManager = ravenManager;
    }

    public void OnboardTenant(string tenantId, string[] urls, string database)
    {
        _ravenManager.AddCluster(tenantId, new RavenStoreOptions
        {
            Urls = urls,
            DefaultDatabase = database
        });
    }

    public void OffboardTenant(string tenantId)
    {
        _ravenManager.RemoveCluster(tenantId);
    }

    public async Task<List<Order>> GetOrders(string tenantId)
    {
        var session = _ravenManager.GetAsyncSession(tenantId);
        return await session.Query<Order>().ToListAsync();
    }
}
```

**Available methods:**

| Method | Description |
|--------|-------------|
| `AddCluster(key, options)` | Add a new cluster at runtime (returns `false` if key exists) |
| `RemoveCluster(key)` | Remove cluster and dispose its store |
| `GetClusterKeys()` | Get all configured cluster keys |
| `HasCluster(key)` | Check if a cluster exists |
| `IsStoreInitialized(key)` | Check if a store has been created for a cluster |

## Extensibility

The library is fully modular. Every component can be replaced with your own implementation, so you can customize any part of the RavenDB integration without forking the library.

### Replaceable Components

| Component | Interface | Default | How to Replace |
|-----------|-----------|---------|----------------|
| **Manager** | `IRavenManager` | `RavenManager` | `AddRavenManager<MyManager>()` |
| **Session Factory** | `IRavenSessionFactory` | `RavenSessionFactory` | Register your own via DI |
| **Document Store** | `IDocumentStore` | RavenDB `DocumentStore` | Register your own singleton via DI |
| **Async Session** | `IAsyncDocumentSession` | RavenDB session | Register your own scoped service via DI |
| **Sync Session** | `IDocumentSession` | RavenDB session | Register your own scoped service via DI |

### Custom RavenManager

You can replace the default `RavenManager` with your own implementation using the generic overloads of `AddRavenManager`:

```csharp
public class MyRavenManager : IRavenManager
{
    // Implement all interface methods with custom logic
    // e.g., custom logging, metrics, tenant resolution, etc.
}

// Register with the default options class
builder.Services.AddRavenManager<MyRavenManager>(options =>
{
    options.DefaultCluster = "Production";
    options.AddCluster("Production", new RavenStoreOptions { ... });
});
```

You can also use a custom options class:

```csharp
public class MyManagerOptions
{
    public string TenantHeader { get; set; }
    public Dictionary<string, RavenStoreOptions> Tenants { get; set; }
}

builder.Services.AddRavenManager<MyRavenManager, MyManagerOptions>(options =>
{
    options.TenantHeader = "X-Tenant-Id";
    options.Tenants = new() { ["acme"] = new RavenStoreOptions { ... } };
});
```

### Custom Session Factory

Replace the default `IRavenSessionFactory` to control how sessions are created:

```csharp
public class MySessionFactory : IRavenSessionFactory
{
    // Add custom behavior: logging, multi-tenancy, session pooling, etc.
}

builder.Services.AddScoped<IRavenSessionFactory, MySessionFactory>();
```

### Custom Session Registration

Override the default session registration to add custom behavior:

```csharp
builder.Services.AddRavenStore(options => { ... });

// Replace the default session registration with your own
builder.Services.AddScoped<IAsyncDocumentSession>(provider =>
{
    var store = provider.GetRequiredService<IDocumentStore>();
    var session = store.OpenAsyncSession();
    // Custom configuration: enable optimistic concurrency, set max requests, etc.
    session.Advanced.MaxNumberOfRequestsPerSession = 100;
    return session;
});
```

## Samples

The repository includes complete working examples in the `samples/` directory:

- **DependencyInjectionSample.Mvc** -- ASP.NET Core MVC app demonstrating `AddRavenManager` with configuration-based setup, session injection in controllers, and store access
- **DependencyInjectionSample.Console** -- Console application demonstrating how to use the same DI extensions with the Generic Host (`Microsoft.Extensions.Hosting`), manual scope management, and service injection

## Development Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later
- [Docker](https://www.docker.com/get-started) (for running RavenDB locally)

### Getting Started

1. Start RavenDB:

```shell
docker compose up -d
```

> Or without docker-compose: `docker run -p 8080:8080 -e RAVEN_Security_UnsecuredAccessAllowed=PublicNetwork ravendb/ravendb`

2. RavenDB Studio is available at [http://localhost:8080](http://localhost:8080)

3. Run a sample:

```shell
dotnet run --project samples/DependencyInjectionSample.Mvc
dotnet run --project samples/DependencyInjectionSample.Console
```

4. Run tests:

```shell
dotnet test
```

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

# RavenDB.AspNetCore.DependencyInjection
Dependency Injection package for using RavenDB with ASP.NET Core.

[![Docker Stars](https://img.shields.io/nuget/v/RavenDB.AspNetCore.DependencyInjection.svg?style=flat)](https://www.nuget.org/packages/RavenDB.AspNetCore.DependencyInjection/)
[![Docker Pulls](https://img.shields.io/nuget/vpre/RavenDB.AspNetCore.DependencyInjection.svg?style=flat)](https://www.nuget.org/packages/RavenDB.AspNetCore.DependencyInjection/)

This package handles the injection of `DocumentSession` (or `AsyncDocumentSession`) for you and while keeping track and managing the DocumentStore(s) for you.

## Getting Started:
Install the [RavenDB.AspNetCore.DependencyInjection](https://www.nuget.org/packages/RavenDB.AspNetCore.DependencyInjection) library through [NuGet](https://nuget.org).
```
    Install-Package RavenDB.AspNetCore.DependencyInjection
    
    Or
    
    Install-Package RavenDB.AspNetCore.DependencyInjection -Pre
```    

## Usage:   

You can now configure the `RavenManager` service in your Startup.cs:

### Use default options from configuration

Pass in a `IConfiguration` to automatically map the values to a `RavenManagerOptions` object.


```csharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
...
	
  services.AddRavenManager(Configuration.GetSection("Raven"))
    .AddScopedAsyncSession();
	
...
}
```

You can specify the default options via configuration, for example in your `appsettings.json` files:

```json
{
    "Raven": {
        "Main": {
            "Url": "{server url}",
            "Database": "{default database}"
        }
    }
}
```

### Use default server server configuration

If you're only using one Raven server, you can configure a single server's options.

```csharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
...
	
  services.AddRavenManagerWithDefaultServer(options => {
      options.Url = "{server url}";
      options.Database = "{database name}";
  })
    .AddScopedAsyncSession();
	
...
}
```

### Configure full RavenManagerOptions

If you need complete control over the RavenManager, you can configure its options.

```csharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
...
	
  services.AddRavenManager(
      options =>
      {
          options.DefaultServer = "Main";
          options.AddServer("Main", new RavenServerOptions()
          {
              Url = "{server url}",
              Database = "{database name}"
          });
      }).AddScopedAsyncSession();
	
...
}
```

Now you can use the standard injection syntax to get you're session or the raven manager:
```csharp
public class HomeController
      : Controller
  {
      private readonly IAsyncDocumentSession _session;
      private readonly IRavenManager _ravenManager;

      public HomeController(
          IAsyncDocumentSession session,
          IRavenManager ravenManager)
      {
          _session = session;
          _ravenManager = ravenManager;
      }
  }
```

## Options

**RavenManagerOptions**

You can configure a list of RavenDB servers with friendly names and options.

- **DefaultServer** - The default name of the Raven server to connect to
- **DefaultConventions** - Default RavenDB document conventions, see [Document Conventions](https://ravendb.net/docs/article-page/3.5/csharp/client-api/configuration/conventions/what-are-conventions)
- **Servers** - A dictionary of server names and server options
- **AddServer(string serverName, RavenServerOptions options)** - Add a new server with a friendly name and options

**RavenServerOptions**

- **Url** - The URL to connect to the Raven server
- **Database** - The database name to connect to
- **Conventions** - Any override document conventions

# User Feedback

## Issues

If you have any problems with or questions about this image, please contact us through a [GitHub issue](https://github.com/FriendlyAgent/RavenDB.AspNetCore.DependencyInjection/issues).
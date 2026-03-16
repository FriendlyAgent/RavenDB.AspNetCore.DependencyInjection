using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System.Collections.Generic;
using Xunit;

namespace RavenDB.AspNetCore.DependencyInjection.Tests
{
    public class RavenManagerServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddRavenManager_WithAction_RegistersRavenManager()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddRavenManager(options =>
            {
                options.DefaultCluster = "cluster1";
                options.AddCluster("cluster1", new RavenStoreOptions
                {
                    Urls = ["http://localhost:8080"],
                    DefaultDatabase = "TestDB"
                });
            });

            var provider = services.BuildServiceProvider();

            // Assert
            var manager = provider.GetService<IRavenManager>();
            Assert.NotNull(manager);
            Assert.Equal("cluster1", manager.DefaultCluster);
        }

        [Fact]
        public void AddRavenManager_WithConfiguration_RegistersRavenManager()
        {
            // Arrange
            var services = new ServiceCollection();
            var configDict = new Dictionary<string, string>
            {
                { "DefaultCluster", "cluster1" },
                { "Clusters:cluster1:Urls:0", "http://localhost:8080" },
                { "Clusters:cluster1:DefaultDatabase", "TestDB" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();

            // Act
            services.AddRavenManager(configuration);
            var provider = services.BuildServiceProvider();

            // Assert
            var manager = provider.GetService<IRavenManager>();
            Assert.NotNull(manager);
            Assert.Equal("cluster1", manager.DefaultCluster);
        }

        [Fact]
        public void AddRavenManager_RegistersAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddRavenManager(options =>
            {
                options.DefaultCluster = "cluster1";
                options.AddCluster("cluster1", new RavenStoreOptions
                {
                    Urls = new[] { "http://localhost:8080" }
                });
            });

            // Assert - Manager should be Singleton to manage DocumentStores
            var descriptor = services[services.Count - 1];
            Assert.Equal(typeof(IRavenManager), descriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        [Fact]
        public void AddScopedAsyncSession_RegistersAsyncSession()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddRavenManager(options =>
            {
                options.DefaultCluster = "cluster1";
                options.AddCluster("cluster1", new RavenStoreOptions
                {
                    Urls = new[] { "http://localhost:8080" },
                    DefaultDatabase = "TestDB"
                });
            }).AddScopedAsyncSession();

            // Assert - check that the async session service is registered
            var sessionDescriptor = services[services.Count - 1];
            Assert.Equal(typeof(Raven.Client.Documents.Session.IAsyncDocumentSession), sessionDescriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, sessionDescriptor.Lifetime);
        }

        [Fact]
        public void AddScopedSession_RegistersSyncSession()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddRavenManager(options =>
            {
                options.DefaultCluster = "cluster1";
                options.AddCluster("cluster1", new RavenStoreOptions
                {
                    Urls = ["http://localhost:8080"],
                    DefaultDatabase = "TestDB"
                });
            }).AddScopedSession();

            // Assert - check that the sync session service is registered
            var sessionDescriptor = services[services.Count - 1];
            Assert.Equal(typeof(Raven.Client.Documents.Session.IDocumentSession), sessionDescriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, sessionDescriptor.Lifetime);
        }

    }
}

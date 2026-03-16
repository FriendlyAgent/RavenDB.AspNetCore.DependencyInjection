using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RavenDB.AspNetCore.DependencyInjection.Tests
{
    public class RavenStoreServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddRavenStore_WithAction_RegistersDocumentStore()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddRavenStore(options =>
            {
                options.Urls = new[] { "http://localhost:8080" };
                options.DefaultDatabase = "TestDB";
            });

            // Assert
            var provider = services.BuildServiceProvider();
            var store = provider.GetService<Raven.Client.Documents.IDocumentStore>();
            Assert.NotNull(store);
        }

        [Fact]
        public void AddRavenStore_WithConfiguration_RegistersDocumentStore()
        {
            // Arrange
            var services = new ServiceCollection();
            var configDict = new Dictionary<string, string>
            {
                { "Urls:0", "http://localhost:8080" },
                { "DefaultDatabase", "TestDB" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();

            // Act
            services.AddRavenStore(configuration);

            // Assert
            var provider = services.BuildServiceProvider();
            var store = provider.GetService<Raven.Client.Documents.IDocumentStore>();
            Assert.NotNull(store);
        }

        [Fact]
        public void AddRavenStore_RegistersAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddRavenStore(options =>
            {
                options.Urls = new[] { "http://localhost:8080" };
            });

            // Assert
            var descriptor = services[0];
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        [Fact]
        public void AddRavenStore_ReturnsBuilder()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var builder = services.AddRavenStore(options =>
            {
                options.Urls = new[] { "http://localhost:8080" };
            });

            // Assert
            Assert.NotNull(builder);
            Assert.IsType<RavenStoreBuilder>(builder);
        }

        [Fact]
        public void AddScopedAsyncSession_RegistersAsyncSession()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddRavenStore(options =>
            {
                options.Urls = new[] { "http://localhost:8080" };
                options.DefaultDatabase = "TestDB";
            }).AddScopedAsyncSession();

            // Assert
            var sessionDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(Raven.Client.Documents.Session.IAsyncDocumentSession));
            Assert.NotNull(sessionDescriptor);
            Assert.Equal(ServiceLifetime.Scoped, sessionDescriptor.Lifetime);
        }

        [Fact]
        public void AddScopedSession_RegistersSyncSession()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddRavenStore(options =>
            {
                options.Urls = new[] { "http://localhost:8080" };
                options.DefaultDatabase = "TestDB";
            }).AddScopedSession();

            // Assert
            var sessionDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(Raven.Client.Documents.Session.IDocumentSession));
            Assert.NotNull(sessionDescriptor);
            Assert.Equal(ServiceLifetime.Scoped, sessionDescriptor.Lifetime);
        }

    }
}

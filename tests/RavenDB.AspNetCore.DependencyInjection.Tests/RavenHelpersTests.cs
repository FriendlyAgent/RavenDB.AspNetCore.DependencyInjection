using RavenDB.AspNetCore.DependencyInjection.Helpers;
using RavenDB.AspNetCore.DependencyInjection.Options;
using Raven.Client.Documents.Conventions;
using Xunit;

namespace RavenDB.AspNetCore.DependencyInjection.Tests
{
    public class RavenHelpersTests
    {

        [Fact]
        public void CreateDocumentStore_WithNullDatabase_UsesEmptyString()
        {
            // Arrange
            var options = new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" },
                DefaultDatabase = null
            };

            // Act
            var store = RavenHelpers.CreateDocumentStore(options);

            // Assert - The key behavior: null becomes empty string
            Assert.Equal("", store.Database);
        }

        [Fact]
        public void CreateDocumentStore_WithStoreConventions_UsesStoreConventions()
        {
            // Arrange
            var storeConventions = new DocumentConventions { MaxNumberOfRequestsPerSession = 100 };
            var defaultConventions = new DocumentConventions { MaxNumberOfRequestsPerSession = 50 };

            var options = new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" },
                Conventions = storeConventions
            };

            // Act
            var store = RavenHelpers.CreateDocumentStore(options, defaultConventions);

            // Assert
            Assert.Equal(100, store.Conventions.MaxNumberOfRequestsPerSession);
        }

        [Fact]
        public void CreateDocumentStore_WithoutStoreConventions_UsesDefaultConventions()
        {
            // Arrange
            var defaultConventions = new DocumentConventions { MaxNumberOfRequestsPerSession = 50 };

            var options = new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" }
            };

            // Act
            var store = RavenHelpers.CreateDocumentStore(options, defaultConventions);

            // Assert
            Assert.Equal(50, store.Conventions.MaxNumberOfRequestsPerSession);
        }

        [Fact]
        public void CreateDocumentStore_WithoutAnyCertificate_HasNullCertificate()
        {
            // Arrange
            var options = new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" }
            };

            // Act
            var store = RavenHelpers.CreateDocumentStore(options);

            // Assert
            Assert.Null(store.Certificate);
        }
    }
}

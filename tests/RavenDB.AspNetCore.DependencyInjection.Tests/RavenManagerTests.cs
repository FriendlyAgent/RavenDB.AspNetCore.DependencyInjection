using Microsoft.Extensions.Options;
using RavenDB.AspNetCore.DependencyInjection.Exceptions;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using MsOptions = Microsoft.Extensions.Options.Options;

namespace RavenDB.AspNetCore.DependencyInjection.Tests
{
    public class RavenManagerTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RavenManager(null));
        }

        [Fact]
        public void Constructor_WithValidOptions_InitializesCorrectly()
        {
            // Arrange
            var options = CreateDefaultOptions();

            // Act
            var manager = new RavenManager(options);

            // Assert
            Assert.NotNull(manager);
            Assert.Equal("cluster1", manager.DefaultCluster);
        }

        [Fact]
        public void Constructor_WithNullClusters_InitializesEmptyDictionary()
        {
            // Arrange
            var options = MsOptions.Create(new RavenManagerOptions
            {
                Clusters = null
            });

            // Act
            var manager = new RavenManager(options);

            // Assert
            Assert.NotNull(manager);
        }

        #endregion

        #region AddCluster Tests

        [Fact]
        public void AddCluster_WithNullClusterKey_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();
            var storeOptions = new RavenStoreOptions();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.AddCluster(null, storeOptions));
        }

        [Fact]
        public void AddCluster_WithNullStoreOptions_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.AddCluster("test", null));
        }

        [Fact]
        public void AddCluster_WithNewCluster_ReturnsTrue()
        {
            // Arrange
            var manager = CreateManager();
            var storeOptions = new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" },
                DefaultDatabase = "TestDB"
            };

            // Act
            var result = manager.AddCluster("newCluster", storeOptions);

            // Assert
            Assert.True(result);
            Assert.True(manager.HasCluster("newCluster"));
        }

        [Fact]
        public void AddCluster_WithExistingCluster_ReturnsFalse()
        {
            // Arrange
            var manager = CreateManager();
            var storeOptions = new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" },
                DefaultDatabase = "TestDB"
            };

            manager.AddCluster("cluster1", storeOptions);

            // Act
            var result = manager.AddCluster("cluster1", storeOptions);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region RemoveCluster Tests

        [Fact]
        public void RemoveCluster_WithNullClusterKey_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.RemoveCluster(null));
        }

        [Fact]
        public void RemoveCluster_WithExistingCluster_ReturnsTrue()
        {
            // Arrange
            var manager = CreateManager();
            var storeOptions = new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" },
                DefaultDatabase = "TestDB"
            };

            manager.AddCluster("toRemove", storeOptions);

            // Act
            var result = manager.RemoveCluster("toRemove");

            // Assert
            Assert.True(result);
            Assert.False(manager.HasCluster("toRemove"));
        }

        [Fact]
        public void RemoveCluster_WithNonExistentCluster_ReturnsFalse()
        {
            // Arrange
            var manager = CreateManager();

            // Act
            var result = manager.RemoveCluster("nonExistent");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region HasCluster Tests

        [Fact]
        public void HasCluster_WithNullClusterKey_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.HasCluster(null));
        }

        [Fact]
        public void HasCluster_WithExistingCluster_ReturnsTrue()
        {
            // Arrange
            var manager = CreateManager();

            // Act
            var result = manager.HasCluster("cluster1");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasCluster_WithNonExistentCluster_ReturnsFalse()
        {
            // Arrange
            var manager = CreateManager();

            // Act
            var result = manager.HasCluster("nonExistent");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetClusterKeys Tests

        [Fact]
        public void GetClusterKeys_ReturnsAllConfiguredClusters()
        {
            // Arrange
            var manager = CreateManager();
            manager.AddCluster("cluster2", new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" },
                DefaultDatabase = "DB2"
            });

            // Act
            var keys = manager.GetClusterKeys().ToList();

            // Assert
            Assert.Contains("cluster1", keys);
            Assert.Contains("cluster2", keys);
        }

        [Fact]
        public void GetClusterKeys_WithNoClusters_ReturnsEmptyCollection()
        {
            // Arrange
            var options = MsOptions.Create(new RavenManagerOptions());
            var manager = new RavenManager(options);

            // Act
            var keys = manager.GetClusterKeys().ToList();

            // Assert
            Assert.Empty(keys);
        }

        #endregion

        #region IsStoreInitialized Tests

        [Fact]
        public void IsStoreInitialized_WithNullClusterKey_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.IsStoreInitialized(null));
        }

        [Fact]
        public void IsStoreInitialized_BeforeStoreAccess_ReturnsFalse()
        {
            // Arrange
            var manager = CreateManager();

            // Act
            var result = manager.IsStoreInitialized("cluster1");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsStoreInitialized_WithNonExistentCluster_ReturnsFalse()
        {
            // Arrange
            var manager = CreateManager();

            // Act
            var result = manager.IsStoreInitialized("nonExistent");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetStore Error Tests

        [Fact]
        public void GetStore_WithNullClusterKey_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.GetStore(null));
        }

        [Fact]
        public void GetStore_WithNonExistentCluster_ThrowsUnknownClusterException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            var ex = Assert.Throws<UnknownClusterException>(() => manager.GetStore("nonExistent"));
            Assert.Contains("nonExistent", ex.Message);
            Assert.Contains("Available clusters:", ex.Message);
        }

        [Fact]
        public void GetStore_WithNoDefaultCluster_ThrowsUnknownClusterException()
        {
            // Arrange
            var options = MsOptions.Create(new RavenManagerOptions
            {
                DefaultCluster = null
            });
            var manager = new RavenManager(options);

            // Act & Assert
            var ex = Assert.Throws<UnknownClusterException>(() => manager.GetStore());
            Assert.Contains("No default cluster", ex.Message);
        }

        #endregion

        #region Session Error Tests

        [Fact]
        public void GetAsyncSession_WithNoDefaultCluster_ThrowsUnknownClusterException()
        {
            // Arrange
            var options = MsOptions.Create(new RavenManagerOptions
            {
                DefaultCluster = null
            });
            var manager = new RavenManager(options);

            // Act & Assert
            var ex = Assert.Throws<UnknownClusterException>(() => manager.GetAsyncSession());
            Assert.Contains("No default cluster", ex.Message);
        }

        [Fact]
        public void GetAsyncSession_WithNullClusterKey_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.GetAsyncSession((string)null));
        }

        [Fact]
        public void GetAsyncSession_WithNullClusterKeyAndDatabase_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.GetAsyncSession(null, "TestDB"));
        }

        [Fact]
        public void GetSession_WithNoDefaultCluster_ThrowsUnknownClusterException()
        {
            // Arrange
            var options = MsOptions.Create(new RavenManagerOptions
            {
                DefaultCluster = null
            });
            var manager = new RavenManager(options);

            // Act & Assert
            var ex = Assert.Throws<UnknownClusterException>(() => manager.GetSession());
            Assert.Contains("No default cluster", ex.Message);
        }

        [Fact]
        public void GetSession_WithNullClusterKey_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.GetSession((string)null));
        }

        [Fact]
        public void GetSession_WithNullClusterKeyAndDatabase_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.GetSession(null, "TestDB"));
        }

        #endregion

        #region Disposal Tests

        [Fact]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            manager.Dispose();
            manager.Dispose(); // Should not throw
        }

        [Fact]
        public void Dispose_AfterDisposal_ThrowsObjectDisposedException()
        {
            // Arrange
            var manager = CreateManager();
            manager.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => manager.AddCluster("test", new RavenStoreOptions()));
            Assert.Throws<ObjectDisposedException>(() => manager.RemoveCluster("cluster1"));
            Assert.Throws<ObjectDisposedException>(() => manager.HasCluster("cluster1"));
            Assert.Throws<ObjectDisposedException>(() => manager.GetClusterKeys());
            Assert.Throws<ObjectDisposedException>(() => manager.IsStoreInitialized("cluster1"));
        }

        [Fact]
        public async Task DisposeAsync_CalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            await manager.DisposeAsync();
            await manager.DisposeAsync(); // Should not throw
        }

        [Fact]
        public async Task DisposeAsync_AfterDisposal_ThrowsObjectDisposedException()
        {
            // Arrange
            var manager = CreateManager();
            await manager.DisposeAsync();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => manager.AddCluster("test", new RavenStoreOptions()));
        }

        #endregion

        #region Helper Methods

        private RavenManager CreateManager()
        {
            return new RavenManager(CreateDefaultOptions());
        }

        private IOptions<RavenManagerOptions> CreateDefaultOptions()
        {
            var managerOptions = new RavenManagerOptions
            {
                DefaultCluster = "cluster1"
            };

            managerOptions.AddCluster("cluster1", new RavenStoreOptions
            {
                Urls = new[] { "http://localhost:8080" },
                DefaultDatabase = "TestDB"
            });

            return MsOptions.Create(managerOptions);
        }

        #endregion
    }
}

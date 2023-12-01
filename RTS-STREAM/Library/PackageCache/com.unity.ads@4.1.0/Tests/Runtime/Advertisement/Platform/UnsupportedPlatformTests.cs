#if NUGET_MOQ_AVAILABLE && UNITY_EDITOR
using Moq;
using NUnit.Framework;
using UnityEngine.Advertisements.Platform;
using UnityEngine.Advertisements.Platform.Unsupported;

namespace UnityEngine.Advertisements.Tests
{
    [TestFixture]
    public class UnsupportedPlatformTests
    {
        [Test]
        public void InitialState()
        {
            var platform = new UnsupportedPlatform();
            Assert.That(platform.GetDebugMode, Is.False, "DebugMode should be set to false by default");
            Assert.That(platform.IsInitialized, Is.False, "IsInitialized should be set to false by default");
            Assert.That(platform.GetVersion(), Is.EqualTo("UnsupportedPlatformVersion"));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("test")]
        [TestCase("ads")]
        [TestCase("1")]
        public void Load(string placementId)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.Load(placementId, null));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("test")]
        [TestCase("ads")]
        [TestCase("1")]
        public void Show(string placementId)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.Show(placementId, null));
        }

        [TestCase(null, false)]
        [TestCase(null, true)]
        [TestCase("", false)]
        [TestCase("", true)]
        [TestCase("test", false)]
        [TestCase("test", true)]
        [TestCase("123435", false)]
        [TestCase("123435", true)]
        public void Initialize(string gameId, bool testMode)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.Initialize(gameId, testMode, null));
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void SetMetaData(string metaDataCategory)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.SetMetaData(new MetaData(metaDataCategory)));
        }

        [Test]
        public void SetupPlatform()
        {
            var unsupportedPlatform = new UnsupportedPlatform();
            var platform = new Mock<IPlatform>();

            Assert.DoesNotThrow(() => unsupportedPlatform.SetupPlatform(platform.Object));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void SetDebugMode(bool debugMode)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.SetDebugMode(debugMode));
        }
    }
}
#endif

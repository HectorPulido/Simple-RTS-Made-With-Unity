#if NUGET_MOQ_AVAILABLE && UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine.Advertisements.Utilities;
using UnityEngine.TestTools;

namespace UnityEngine.Advertisements.Tests
{
    public class PlatformTests
    {
        private Mock<IUnityLifecycleManager> m_CoroutineExecutorMock;
        private Mock<INativePlatform> m_NativePlatformMock;
        private Mock<IBanner> m_BannerMock;

        [SetUp]
        public void Setup()
        {
            m_CoroutineExecutorMock = new Mock<IUnityLifecycleManager>(MockBehavior.Strict);
            m_NativePlatformMock = new Mock<INativePlatform>(MockBehavior.Default);
            m_BannerMock = new Mock<IBanner>(MockBehavior.Default);

            m_CoroutineExecutorMock.Setup(x => x.Post(It.IsAny<Action>())).Callback<Action>((action) => { action?.Invoke(); });
            m_CoroutineExecutorMock.Setup(x => x.StartCoroutine(It.IsAny<IEnumerator>())).Callback<IEnumerator>(x => {
                while (x.MoveNext()) {}
            }).Returns<Coroutine>(null);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void IsInitialize(bool actual, bool expected)
        {
            m_NativePlatformMock.Setup(x => x.IsInitialized()).Returns(actual);
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            Assert.That(platform.IsInitialized, Is.EqualTo(expected), "IsInitialized did not return the correct value");
        }

        [Test]
        [TestCase("3.4.0", "3.4.0", TestName = "GetVersion(Valid String)")]
        [TestCase("", "", TestName = "GetVersion(Empty String)")]
        [TestCase(null, "UnknownVersion", TestName = "GetVersion(null String)")]
        public void GetVersion(string actual, string expected)
        {
            m_NativePlatformMock.Setup(x => x.GetVersion()).Returns(actual);
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            Assert.That(platform.Version, Is.EqualTo(expected), "GetVersion failed to return the correct value");
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void GetDebugMode(bool actual, bool expected)
        {
            m_NativePlatformMock.Setup(x => x.GetDebugMode()).Returns(actual);
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            Assert.That(platform.DebugMode, Is.EqualTo(expected), "GetDebugMode did not return the correct value");
            m_NativePlatformMock.Verify(x => x.GetDebugMode(), Times.Once(), "NativePlatform.GetDebugMode() was not called as expected");
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void SetDebugMode(bool actual, bool expected)
        {
            m_NativePlatformMock.Setup(x => x.SetDebugMode(It.IsAny<bool>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.DebugMode = actual;
            m_NativePlatformMock.Verify(x => x.SetDebugMode(It.IsAny<bool>()), Times.Once(), "NativePlatform.SetDebugMode(bool) was not called as expected");
        }

        [Test]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("1234567", true)]
        [TestCase("1234567", null)]
        [TestCase("1234567", false)]
        [TestCase(null, null)]
        public void NativePlatformInitialize(string gameId, bool testMode)
        {
            m_NativePlatformMock.Setup(x => x.Initialize(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IUnityAdsInitializationListener>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Initialize(gameId, testMode, null);
            m_NativePlatformMock.Verify(x => x.Initialize(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IUnityAdsInitializationListener>()), Times.Once(), "NativePlatform.Initialize() was not called as expected");
        }

        [Test]
        [TestCase("loadAd")]
        public void NativePlatformLoad(string placementId)
        {
            m_NativePlatformMock.Setup(x => x.Load(It.IsAny<string>(), It.IsAny<IUnityAdsLoadListener>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Load(placementId, null);
            m_NativePlatformMock.Verify(x => x.Load(It.IsAny<string>(), It.IsAny<IUnityAdsLoadListener>()), Times.Once(), "NativePlatform.Load() was not called as expected");
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void NativePlatformLoadError(string placementId)
        {
            m_NativePlatformMock.Setup(x => x.Load(It.IsAny<string>(), It.IsAny<IUnityAdsLoadListener>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Load(placementId, null);
            LogAssert.Expect(LogType.Error, "placementId cannot be nil or empty");
            m_NativePlatformMock.Verify(x => x.Load(It.IsAny<string>(), It.IsAny<IUnityAdsLoadListener>()), Times.Never(), "NativePlatform.Load() was called when it should not have been");
        }

        [Test]
        [TestCase("showAd")]
        [TestCase("")]
        [TestCase(null)]
        public void NativePlatformShow(string placementId)
        {
            m_NativePlatformMock.Setup(x => x.Show(It.IsAny<string>(), null));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Show(placementId, null, null);
            m_NativePlatformMock.Verify(x => x.Show(It.IsAny<string>(), null), Times.Once(), "NativePlatform.Show() was not called as expected");
        }


        [Test]
        public void NativePlatformSetMetaData()
        {
            var metaData = new MetaData("TestMeta");
            metaData.Set("key1", "value1");

            m_NativePlatformMock.Setup(x => x.SetMetaData(It.IsAny<MetaData>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.SetMetaData(metaData);
            m_NativePlatformMock.Verify(x => x.SetMetaData(It.IsAny<MetaData>()), Times.Once(), "NativePlatform.SetMetaData() was not called as expected");
        }


        [Test]
        [TestCase("randomPlacementId", UnityAdsShowError.NOT_INITIALIZED)]
        [TestCase("randomPlacementId", UnityAdsShowError.NOT_READY)]
        [TestCase("randomPlacementId", UnityAdsShowError.VIDEO_PLAYER_ERROR)]
        [TestCase("randomPlacementId", UnityAdsShowError.INVALID_ARGUMENT)]
        [TestCase(null, UnityAdsShowError.INVALID_ARGUMENT)]
        [TestCase("randomPlacementId", UnityAdsShowError.NO_CONNECTION)]
        [TestCase("randomPlacementId", UnityAdsShowError.ALREADY_SHOWING)]
        [TestCase("randomPlacementId", UnityAdsShowError.INTERNAL_ERROR)]
        [TestCase("randomPlacementId", UnityAdsShowError.UNKNOWN)]
        public void OnShowFailureCallbackResetsIsShowing(string testPlacementId, UnityAdsShowError testShowResult)
        {
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Initialize("1234567", false, null);
            platform.OnUnityAdsShowStart(testPlacementId);
            Assert.That(platform.IsShowing, Is.True, "IsShowing should be set to true after starting showing an ad");
            platform.OnUnityAdsShowFailure(testPlacementId, testShowResult, "show failed");
            Assert.That(platform.IsShowing, Is.False, "IsShowing should be set to false after finishing showing an ad");
        }

        [Test]
        [TestCase("randomPlacementId", UnityAdsShowCompletionState.SKIPPED)]
        [TestCase(null, UnityAdsShowCompletionState.SKIPPED)]
        [TestCase("randomPlacementId", UnityAdsShowCompletionState.COMPLETED)]
        [TestCase(null, UnityAdsShowCompletionState.COMPLETED)]
        [TestCase("randomPlacementId", UnityAdsShowCompletionState.UNKNOWN)]
        [TestCase(null, UnityAdsShowCompletionState.UNKNOWN)]
        public void OnStartAndCompleteCallbacksToggleIsShowing(string testPlacementId, UnityAdsShowCompletionState testShowResult)
        {
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Initialize("1234567", false, null);
            platform.OnUnityAdsShowStart(testPlacementId);
            Assert.That(platform.IsShowing, Is.True, "IsShowing should be set to true after starting showing an ad");
            platform.OnUnityAdsShowComplete(testPlacementId, testShowResult);
            Assert.That(platform.IsShowing, Is.False, "IsShowing should be set to false after finishing showing an ad");
        }

        [UnityTest]
        [TestCase("randomGamerSid", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator ShowOptionsGamerSid(string expectedGamerSid)
        {
            var setMetaDataCalled = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            m_NativePlatformMock.Setup(x => x.SetMetaData(It.IsAny<MetaData>())).Callback<MetaData>(result => {
                Assert.That(result.category, Is.EqualTo("player"), "The category player should exist if gamerSid was stored properly");
                Assert.That(result.Get("server_id"), Is.EqualTo(expectedGamerSid), "GamerSid was not stored properly in MetaData");
                setMetaDataCalled = true;
            });

            var showOptions = new ShowOptions();
            showOptions.gamerSid += expectedGamerSid;
            platform.Show("placementId", showOptions, null);
            m_NativePlatformMock.Verify(x => x.SetMetaData(It.IsAny<MetaData>()), Times.Once(), "Set MetaData should have been called with the gamerSid");
            while (!setMetaDataCalled) yield return null;
            Assert.That(setMetaDataCalled, Is.True, "The SetMetaData function should have been called");
        }
    }
}
#endif

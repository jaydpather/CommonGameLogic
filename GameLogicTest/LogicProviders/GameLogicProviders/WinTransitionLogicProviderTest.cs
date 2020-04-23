using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders;

namespace GameLogicTest.LogicProviders.GameLogicProviders
{
    [TestClass]
    public class WinTransitionLogicProviderTest : TestBase
    {
        [TestMethod]
        public void OnStart()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var winTransitionSound = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("WinTransitionSound").Returns(winTransitionSound);

            var audioSourceWinTransition = Substitute.For<IAudioSource>();
            winTransitionSound.GetComponent<IAudioSource>().Returns(audioSourceWinTransition);

            var dataLayer = Substitute.For<IDataLayer>();

            var winTransitionLogicProvider = new WinTransitionLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            #endregion

            #region act
            winTransitionLogicProvider.OnStart();
            #endregion

            #region assert
            var privateAudioSource = GetPrivateMember<IAudioSource>("_audioSourceWinTransition", winTransitionLogicProvider);
            Assert.AreEqual(audioSourceWinTransition, privateAudioSource);
            #endregion
        }

        [TestMethod]
        public void OnActivate()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var winTransitionLogicProvider = new WinTransitionLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var audioSourceWinTransition = Substitute.For<IAudioSource>();
            SetPrivateMember("_audioSourceWinTransition", winTransitionLogicProvider, audioSourceWinTransition);
            #endregion

            #region act
            winTransitionLogicProvider.OnActivate();
            #endregion

            #region assert
            audioSourceWinTransition.Received().Play();
            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_StillPlaying()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var winTransitionLogicProvider = new WinTransitionLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var audioSourceWinTransition = Substitute.For<IAudioSource>();
            audioSourceWinTransition.IsPlaying.Returns(true);
            SetPrivateMember("_audioSourceWinTransition", winTransitionLogicProvider, audioSourceWinTransition);
            #endregion

            #region act
            winTransitionLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            gameLogicHandler.DidNotReceive().SetSceneState(Arg.Any<int>());
            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_NotPlaying()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var winTransitionLogicProvider = new WinTransitionLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var audioSourceWinTransition = Substitute.For<IAudioSource>();
            audioSourceWinTransition.IsPlaying.Returns(false);
            SetPrivateMember("_audioSourceWinTransition", winTransitionLogicProvider, audioSourceWinTransition);
            #endregion

            #region act
            winTransitionLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            gameLogicHandler.Received().SetSceneState((int)GameState.Win);
            #endregion
        }
    }
}

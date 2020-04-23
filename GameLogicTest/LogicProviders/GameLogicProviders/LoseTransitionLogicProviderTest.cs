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

namespace GameLogicTest.LogicProviders
{
    [TestClass]
    public class LoseTransitionLogicProviderTest : TestBase
    {
        [TestMethod]
        public void OnStart()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var loseTransitionLogicProvider = new LoseTransitionLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);
            #endregion

            #region act
            loseTransitionLogicProvider.OnStart();
            #endregion

            #region assert
            gameEngineInterface.Received(1).FindGameObject("Explosion");
            gameEngineInterface.Received(1).FindGameObject("PlayerShip");
            #endregion
        }

        [TestMethod]
        public void OnActivate()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var loseTransitionLogicProvider = new LoseTransitionLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var sourceExplosion = Substitute.For<IGameObject>();
            SetPrivateMember("_sourceExplosion", loseTransitionLogicProvider, sourceExplosion);

            var playerShip = Substitute.For<IGameObject>();
            SetPrivateMember("_playerShip", loseTransitionLogicProvider, playerShip);
            #endregion

            #region act
            loseTransitionLogicProvider.OnActivate();
            #endregion

            #region assert
            gameEngineInterface.Received(1).Clone(sourceExplosion, string.Empty);
            playerShip.Received(1).SetActive(false);
            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_StillPlaying()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var loseTransitionLogicProvider = new LoseTransitionLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var audioSourceExplosion = Substitute.For<IAudioSource>();
            audioSourceExplosion.IsPlaying.Returns(true);
            SetPrivateMember("_audioSourceExplosion", loseTransitionLogicProvider, audioSourceExplosion); 
            #endregion

            #region act
            loseTransitionLogicProvider.UpdateGameObjects();
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

            var loseTransitionLogicProvider = new LoseTransitionLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var audioSourceExplosion = Substitute.For<IAudioSource>();
            audioSourceExplosion.IsPlaying.Returns(false);
            SetPrivateMember("_audioSourceExplosion", loseTransitionLogicProvider, audioSourceExplosion);
            #endregion

            #region act
            loseTransitionLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            gameLogicHandler.Received().SetSceneState((int)GameState.Lose);
            #endregion
        }
    }
}

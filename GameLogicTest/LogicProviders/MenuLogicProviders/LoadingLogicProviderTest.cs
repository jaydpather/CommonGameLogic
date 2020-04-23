using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders.MenuLogicProviders;

namespace GameLogicTest.LogicProviders.MenuLogicProviders
{
    [TestClass]
    public class LoadingLogicProviderTest : TestBase
    {
        [TestMethod]
        public void OnStart()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();

            var gameLogicHandler = Substitute.For<ILogicHandler>();


            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var pnlLoading = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("pnlLoading").Returns(pnlLoading);


            var dataLayer = Substitute.For<IDataLayer>();
            
            var loadingLogicProvider = new LoadingLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            #endregion

            #region act
            loadingLogicProvider.OnStart();
            #endregion

            #region assert
            gameEngineInterface.Received().FindGameObject("txtProgressDot0");
            gameEngineInterface.Received().FindGameObject("txtProgressDot1");
            gameEngineInterface.Received().FindGameObject("txtProgressDot2");
            gameEngineInterface.Received().FindGameObject("pnlLoading");
            pnlLoading.Received().SetActive(false);
            #endregion
        }

        [TestMethod]
        public void OnActivate()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();

            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();


            var loadingLogicProvider = new LoadingLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var pnlLoading = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlLoading", loadingLogicProvider, pnlLoading);

            #endregion

            #region act
            loadingLogicProvider.OnActivate();
            #endregion

            #region assert
            pnlLoading.Received().SetActive(true);
            gameController.Received().LoadSceneAsync(Constants.SceneNames.GameScene);
            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_Dot0()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();

            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var loadingLogicProvider = new LoadingLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var txt0 = Substitute.For<IText>();
            var txt1 = Substitute.For<IText>();
            var txt2 = Substitute.For<IText>();
            SetPrivateMember("_txtProgressDot0", loadingLogicProvider, txt0);
            SetPrivateMember("_txtProgressDot1", loadingLogicProvider, txt1);
            SetPrivateMember("_txtProgressDot2", loadingLogicProvider, txt2);

            SetPrivateMember("_timeAccumulated", loadingLogicProvider, 1.0f);
            SetPrivateMember("_redDotIndex", loadingLogicProvider, 2);

            #endregion

            #region act
            loadingLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            txt0.Received().SetColor(1, 0, 0);
            txt2.Received().SetColor(1, 1, 1);
            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_Dot1()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();

            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var loadingLogicProvider = new LoadingLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var txt0 = Substitute.For<IText>();
            var txt1 = Substitute.For<IText>();
            var txt2 = Substitute.For<IText>();
            SetPrivateMember("_txtProgressDot0", loadingLogicProvider, txt0);
            SetPrivateMember("_txtProgressDot1", loadingLogicProvider, txt1);
            SetPrivateMember("_txtProgressDot2", loadingLogicProvider, txt2);

            SetPrivateMember("_timeAccumulated", loadingLogicProvider, 1.0f);
            SetPrivateMember("_redDotIndex", loadingLogicProvider, 0);

            #endregion

            #region act
            loadingLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            txt1.Received().SetColor(1, 0, 0);
            txt0.Received().SetColor(1, 1, 1);
            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_Dot2()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();

            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var loadingLogicProvider = new LoadingLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var txt0 = Substitute.For<IText>();
            var txt1 = Substitute.For<IText>();
            var txt2 = Substitute.For<IText>();
            SetPrivateMember("_txtProgressDot0", loadingLogicProvider, txt0);
            SetPrivateMember("_txtProgressDot1", loadingLogicProvider, txt1);
            SetPrivateMember("_txtProgressDot2", loadingLogicProvider, txt2);

            SetPrivateMember("_timeAccumulated", loadingLogicProvider, 1.0f);
            SetPrivateMember("_redDotIndex", loadingLogicProvider, 1);

            #endregion

            #region act
            loadingLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            txt2.Received().SetColor(1, 0, 0);
            txt1.Received().SetColor(1, 1, 1);
            #endregion
        }
    }
}

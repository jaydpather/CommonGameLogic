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
using static ThirdEyeSoftware.GameLogic.LogicProviders.BaseLogicProvider;

namespace GameLogicTest.LogicProviders
{
    [TestClass]
    public class LoseLogicProviderTest : TestBase
    {
        [TestMethod]
        public void OnStart()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            //Game Over UI controls:
            var pnlGameOver = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("pnlGameOver").Returns(pnlGameOver);

            var btnGameOverMainMenu = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("btnGameOverMainMenu").Returns(btnGameOverMainMenu);

            var btnGameOverGetMoreLives = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("btnGameOverGetMoreLives").Returns(btnGameOverGetMoreLives);

            //todo Post-UT: shouldn't there be a different logic provider for game lose VS game over? is it worth the time to refactor?

            //Game Lose UI controls:
            var btnGameLoseMainMenu = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("btnGameLoseMainMenu").Returns(btnGameLoseMainMenu);

            var btnGameLoseTryAgain = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("btnGameLoseTryAgain").Returns(btnGameLoseTryAgain);

            var pnlGameLose = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("pnlGameLose").Returns(pnlGameLose);

            var dataLayer = Substitute.For<IDataLayer>();

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);
            #endregion

            #region act
            loseLogicProvider.OnStart();
            #endregion

            #region assert
            Assert.IsNotNull(btnGameOverMainMenu.LogicHandler);
            Assert.IsNotNull(btnGameOverGetMoreLives.LogicHandler);
            pnlGameOver.Received().SetActive(false);

            Assert.IsNotNull(btnGameLoseMainMenu.LogicHandler);
            Assert.IsNotNull(btnGameLoseTryAgain.LogicHandler);
            pnlGameLose.Received().SetActive(false);

            #endregion
        }

        [TestMethod]
        public void OnActivate_OutOfLives()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();

            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();
            gameEngineInterface.AppStoreService.LogToDebugOutput = null;

            var dataLayer = Substitute.For<IDataLayer>();
            dataLayer.GetNumLivesRemaining().Returns(0);

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var pnlGameOver = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlGameOver", loseLogicProvider, pnlGameOver);
            #endregion

            #region act
            loseLogicProvider.OnActivate();
            #endregion

            #region assert
            pnlGameOver.Received().SetActive(true);
            Assert.IsNotNull(gameEngineInterface.AppStoreService.LogToDebugOutput);
            #endregion
        }

        [TestMethod]
        public void OnActivate_PlayerHasLives()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();
            gameEngineInterface.AppStoreService.LogToDebugOutput = null;

            var dataLayer = Substitute.For<IDataLayer>();
            dataLayer.GetNumLivesRemaining().Returns(1);

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var pnlGameLose = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlGameLose", loseLogicProvider, pnlGameLose);
            #endregion

            #region act
            loseLogicProvider.OnActivate();
            #endregion

            #region assert
            pnlGameLose.Received().SetActive(true);
            Assert.IsNotNull(gameEngineInterface.AppStoreService.LogToDebugOutput);
            #endregion
        }

        [TestMethod]
        public void OnClick()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();

            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();


            var dataLayer = Substitute.For<IDataLayer>();

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            #endregion

            #region act
            loseLogicProvider.OnClick("btnGameOverMainMenu");
            #endregion

            #region assert
            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", loseLogicProvider);
            Assert.IsTrue(uiInputStates[UIInputAxis.btnGameOverMainMenu]);
            #endregion
        }

        [TestMethod]
        public void HandleInput_GameOverMainMenu()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", loseLogicProvider);
            uiInputStates[UIInputAxis.btnGameOverMainMenu] = true;
            uiInputStates[UIInputAxis.btnGameOverGetMoreLives] = false;
            uiInputStates[UIInputAxis.btnGameLoseMainMenu] = false;
            uiInputStates[UIInputAxis.btnGameLoseTryAgain] = false;
            #endregion

            #region act
            loseLogicProvider.HandleInput();
            #endregion

            #region assert
            Assert.IsFalse(uiInputStates[UIInputAxis.btnGameOverMainMenu]);
            gameLogicHandler.Received(1).SetAppState(AppState.MainMenu);
            #endregion
        }

        [TestMethod]
        public void HandleInput_GameOverGetMoreLives()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", loseLogicProvider);
            uiInputStates[UIInputAxis.btnGameOverMainMenu] = false;
            uiInputStates[UIInputAxis.btnGameOverGetMoreLives] = true;
            uiInputStates[UIInputAxis.btnGameLoseMainMenu] = false;
            uiInputStates[UIInputAxis.btnGameLoseTryAgain] = false;
            #endregion

            #region act
            loseLogicProvider.HandleInput();
            #endregion

            #region assert
            Assert.IsFalse(uiInputStates[UIInputAxis.btnGameOverGetMoreLives]);
            gameEngineInterface.AppStoreService.Received(1).BuyProductByID(Constants.ProductNames.BuyLivesSmall); //todo: update test. this now displays the Get More Lives dialog instead of just making a purchase
            #endregion
        }

        [TestMethod]
        public void HandleInput_GameLoseMainMenu()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", loseLogicProvider);
            uiInputStates[UIInputAxis.btnGameOverMainMenu] = false;
            uiInputStates[UIInputAxis.btnGameOverGetMoreLives] = false;
            uiInputStates[UIInputAxis.btnGameLoseMainMenu] = true;
            uiInputStates[UIInputAxis.btnGameLoseTryAgain] = false;
            #endregion

            #region act
            loseLogicProvider.HandleInput();
            #endregion

            #region assert
            Assert.IsFalse(uiInputStates[UIInputAxis.btnGameLoseMainMenu]);
            gameLogicHandler.Received(1).SetAppState(AppState.MainMenu);
            #endregion
        }

        [TestMethod]
        public void HandleInput_GameLoseTryAgain()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", loseLogicProvider);
            uiInputStates[UIInputAxis.btnGameOverMainMenu] = false;
            uiInputStates[UIInputAxis.btnGameOverGetMoreLives] = false;
            uiInputStates[UIInputAxis.btnGameLoseMainMenu] = false;
            uiInputStates[UIInputAxis.btnGameLoseTryAgain] = true;
            #endregion

            #region act
            loseLogicProvider.HandleInput();
            #endregion

            #region assert
            Assert.IsFalse(uiInputStates[UIInputAxis.btnGameLoseTryAgain]);
            gameController.Received(1).LoadSceneAsync(Constants.SceneNames.GameScene);
            #endregion
        }

        [TestMethod]
        public void OnDeactivate()
        {
            #region arrange
            var gameController = Substitute.For<IGameController>();
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var loseLogicProvider = new LoseLogicProvider(gameController, gameLogicHandler, gameEngineInterface, dataLayer);

            var pnlGameLose = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlGameLose", loseLogicProvider, pnlGameLose);

            var pnlGameOver = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlGameOver", loseLogicProvider, pnlGameOver);

            #endregion

            #region act
            loseLogicProvider.OnDeActivate();
            #endregion

            #region assert
            pnlGameLose.Received(1).SetActive(false);
            pnlGameOver.Received(1).SetActive(false);
            #endregion
        }
    }
}

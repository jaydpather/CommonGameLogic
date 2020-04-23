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
    public class PauseLogicProviderTest : TestBase
    {
        [TestMethod]
        public void OnStart()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();


            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var btnResumeGame = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("btnResumeGame").Returns(btnResumeGame);

            var btnQuitGame = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("btnQuitGame").Returns(btnQuitGame);

            var pnlPauseMenu = Substitute.For<IGameObject>();
            gameEngineInterface.FindGameObject("pnlPauseMenu").Returns(pnlPauseMenu);


            var dataLayer = Substitute.For<IDataLayer>();

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            #endregion

            #region act
            pauseLogicProvider.OnStart();
            #endregion

            #region assert
            Assert.AreEqual(gameLogicHandler, btnResumeGame.LogicHandler);
            Assert.AreEqual(gameLogicHandler, btnQuitGame.LogicHandler);
            pnlPauseMenu.Received().SetActive(false);
            #endregion
        }

        [TestMethod]
        public void OnActivate()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            gameEngineInterface.TimeScale = 1;

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var pnlPauseMenu = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlPauseMenu", pauseLogicProvider, pnlPauseMenu);

            #endregion

            #region act
            pauseLogicProvider.OnActivate();
            #endregion

            #region assert
            var timeScaleBackup = GetPrivateMember<float>("_timeScaleBackup", pauseLogicProvider);
            Assert.AreEqual(1, timeScaleBackup);

            Assert.AreEqual(0, gameEngineInterface.TimeScale);

            pnlPauseMenu.Received().SetActive(true);
            #endregion
        }

        [TestMethod]
        public void OnDeActivate_NotNull()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var pnlPauseMenu = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlPauseMenu", pauseLogicProvider, pnlPauseMenu);
            SetPrivateMember("_timeScaleBackup", pauseLogicProvider, 1);
            #endregion

            #region act
            pauseLogicProvider.OnDeActivate();
            #endregion

            #region assert
            pnlPauseMenu.Received(1).SetActive(false);
            Assert.AreEqual(1, gameEngineInterface.TimeScale);
            #endregion
        }

        [TestMethod]
        public void OnDeActivate_Null()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            SetPrivateMember("_pnlPauseMenu", pauseLogicProvider, (IGameObject)null);
            #endregion

            #region act
            pauseLogicProvider.OnDeActivate();
            #endregion

            #region assert
            Assert.IsTrue(true); //if we made it this far without throwing an exception, then we have passed the test.
            #endregion
        }

        [TestMethod]
        public void HandleInput_Cancel()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", pauseLogicProvider);
            inputStates[InputAxis.Cancel] = 1;

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", pauseLogicProvider);
            uiInputStates[UIInputAxis.btnResumeGame] = false;
            uiInputStates[UIInputAxis.btnQuitGame] = false;
            #endregion

            #region act
            pauseLogicProvider.HandleInput();
            #endregion

            #region assert
            gameLogicHandler.Received().SetSceneState((int)GameState.InGame);
            #endregion
        }

        [TestMethod]
        public void HandleInput_btnResumeGame()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", pauseLogicProvider);
            inputStates[InputAxis.Cancel] = 0;

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", pauseLogicProvider);
            uiInputStates[UIInputAxis.btnResumeGame] = true;
            uiInputStates[UIInputAxis.btnQuitGame] = false;
            #endregion

            #region act
            pauseLogicProvider.HandleInput();
            #endregion

            #region assert
            gameLogicHandler.Received().SetSceneState((int)GameState.InGame);
            #endregion
        }

        [TestMethod]
        public void HandleInput_btnQuitGame()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", pauseLogicProvider);
            inputStates[InputAxis.Cancel] = 0;

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", pauseLogicProvider);
            uiInputStates[UIInputAxis.btnResumeGame] = false;
            uiInputStates[UIInputAxis.btnQuitGame] = true;
            #endregion

            #region act
            pauseLogicProvider.HandleInput();
            #endregion

            #region assert
            gameLogicHandler.Received().SetAppState(AppState.MainMenu);
            #endregion
        }

        [TestMethod]
        public void OnClick_btnResumeGame()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", pauseLogicProvider);
            uiInputStates[UIInputAxis.btnResumeGame] = false;
            uiInputStates[UIInputAxis.btnQuitGame] = false;
            #endregion

            #region act
            pauseLogicProvider.OnClick("btnResumeGame");
            #endregion

            #region assert
            Assert.IsTrue(uiInputStates[UIInputAxis.btnResumeGame]);
            Assert.IsFalse(uiInputStates[UIInputAxis.btnQuitGame]);
            #endregion
        }

        [TestMethod]
        public void OnClick_btnQuitGame()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var pauseLogicProvider = new PauseLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", pauseLogicProvider);
            uiInputStates[UIInputAxis.btnResumeGame] = false;
            uiInputStates[UIInputAxis.btnQuitGame] = false;
            #endregion

            #region act
            pauseLogicProvider.OnClick("btnQuitGame");
            #endregion

            #region assert
            Assert.IsFalse(uiInputStates[UIInputAxis.btnResumeGame]);
            Assert.IsTrue(uiInputStates[UIInputAxis.btnQuitGame]);
            #endregion
        }
    }
}

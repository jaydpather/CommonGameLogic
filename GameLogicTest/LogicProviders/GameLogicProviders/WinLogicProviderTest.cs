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
    public class WinLogicProviderTest : TestBase
    {
        private ILogicHandler _logicHandler;
        private IGameEngineInterface _gameEngineInterface;
        private IDataLayer _dataLayer;

        private WinLogicProvider _winLogicProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _logicHandler = Substitute.For<ILogicHandler>();
            _gameEngineInterface = Substitute.For<IGameEngineInterface>();
            _dataLayer = Substitute.For<IDataLayer>();

            _winLogicProvider = new WinLogicProvider(_logicHandler, _gameEngineInterface, _dataLayer);
        }

        [TestMethod]
        public void OnStart()
        {
            #region arrange
            //var gameLogicHandler = Substitute.For<ILogicHandler>();

            //var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var btnWinOk = Substitute.For<IGameObject>();
            var pnlGameWin = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject(Arg.Any<string>()).Returns(btnWinOk, pnlGameWin);

            //var dataLayer = Substitute.For<IDataLayer>();

            //var winLogicProvider = new WinLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            #endregion

            #region act
            _winLogicProvider.OnStart();
            #endregion

            #region assert
            Assert.IsNotNull(btnWinOk.LogicHandler);
            pnlGameWin.Received().SetActive(false);
            _gameEngineInterface.Received(1).FindGameObject("btnGameWinOk");
            _gameEngineInterface.Received(1).FindGameObject("btnGameWinNextLevel");
            _gameEngineInterface.Received(1).FindGameObject("txtWinMessage");
            _gameEngineInterface.Received(1).FindGameObject("pnlGameWin");
            #endregion
        }

        [TestMethod]
        public void OnActivate_BeatRegularLevel()
        {
            #region arrange
            _logicHandler.GameController.JustBeatMaxLevel.Returns(false);
            _logicHandler.GameController.JustSavedLatestLevel.Returns(false);

            var pnlGameWin = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlGameWin", _winLogicProvider, pnlGameWin);

            var txtWinMsg = Substitute.For<IText>();
            SetPrivateMember("_txtWinMessage", _winLogicProvider, txtWinMsg);

            var btnGameWinNextLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnGameWinNextLevel", _winLogicProvider, btnGameWinNextLevel);

            #endregion

            #region act
            _winLogicProvider.OnActivate();
            #endregion

            #region assert
            Assert.AreEqual(Constants.UIMessages.BeatLevel, txtWinMsg.Text);
            btnGameWinNextLevel.Received(1).SetActive(true);
            pnlGameWin.Received().SetActive(true);
            #endregion
        }

        [TestMethod]
        public void OnActivate_BeatRegularLevel_LatestLevel()
        {
            #region arrange
            _logicHandler.GameController.JustBeatMaxLevel.Returns(false);
            _logicHandler.GameController.JustSavedLatestLevel.Returns(true);

            var pnlGameWin = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlGameWin", _winLogicProvider, pnlGameWin);

            var txtWinMsg = Substitute.For<IText>();
            SetPrivateMember("_txtWinMessage", _winLogicProvider, txtWinMsg);

            var btnGameWinNextLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnGameWinNextLevel", _winLogicProvider, btnGameWinNextLevel);

            #endregion

            #region act
            _winLogicProvider.OnActivate();
            #endregion

            #region assert
            var expectedMsg = Constants.UIMessages.BeatLevel + Environment.NewLine + Constants.UIMessages.ProgressSaved;
            Assert.AreEqual(expectedMsg, txtWinMsg.Text);
            btnGameWinNextLevel.Received(1).SetActive(true);
            pnlGameWin.Received().SetActive(true);

            Assert.IsFalse(_logicHandler.GameController.JustSavedLatestLevel); //WinLogicProvider needs to reset this to false. (b/c if the user returns the main menu and beats a lower level, then we shouldn't say we've saved)
            #endregion
        }


        [TestMethod]
        public void OnActivate_BeatMaxLevel()
        {
            #region arrange
            _logicHandler.GameController.JustBeatMaxLevel.Returns(true);
            _logicHandler.GameController.JustSavedLatestLevel.Returns(false);

            var pnlGameWin = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlGameWin", _winLogicProvider, pnlGameWin);

            var txtWinMsg = Substitute.For<IText>();
            SetPrivateMember("_txtWinMessage", _winLogicProvider, txtWinMsg);

            var btnGameWinNextLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnGameWinNextLevel", _winLogicProvider, btnGameWinNextLevel);

            #endregion

            #region act
            _winLogicProvider.OnActivate();
            #endregion

            #region assert
            Assert.AreEqual(Constants.UIMessages.BeatGame, txtWinMsg.Text);
            btnGameWinNextLevel.Received(1).SetActive(false);
            pnlGameWin.Received().SetActive(true);
            #endregion
        }

        [TestMethod]
        public void HandleInput_MainMenu()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();
            
            var winLogicProvider = new WinLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            var uiInputStates = new Dictionary<UIInputAxis, bool>();
            uiInputStates.Add(UIInputAxis.btnGameWinOk, true);
            SetPrivateMember("_uiInputStates", winLogicProvider, uiInputStates);
            #endregion

            #region act
            winLogicProvider.HandleInput();
            #endregion

            #region assert
            Assert.IsFalse(uiInputStates[UIInputAxis.btnGameWinOk]);
            gameLogicHandler.Received().SetAppState(AppState.MainMenu);
            #endregion
        }

        [TestMethod]
        public void btnGameWinNextLevel_Click()
        {
            _logicHandler.GameController = Substitute.For<IGameController>();

            _winLogicProvider.btnGameWinNextLevel_Click();

            _logicHandler.GameController.Received(1).LoadSceneAsync(Constants.SceneNames.GameScene);
        }

        [TestMethod]
        public void OnClick_Ok()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var winLogicProvider = new WinLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);

            #endregion

            #region act
            winLogicProvider.OnClick("btnGameWinOk");
            #endregion

            #region assert
            var uiInputStates = GetPrivateMember<Dictionary<UIInputAxis, bool>>("_uiInputStates", winLogicProvider);
            Assert.IsTrue(uiInputStates[UIInputAxis.btnGameWinOk]);
            #endregion
        }
    }
}

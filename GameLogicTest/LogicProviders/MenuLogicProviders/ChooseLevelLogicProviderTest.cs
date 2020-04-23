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
    public class ChooseLevelLogicProviderTest : TestBase
    {
        ChooseLevelLogicProvider _chooseLevelLogicProvider;
        ILogicHandler _logicHandler;
        IGameEngineInterface _gameEngineInterface;
        IDataLayer _dataLayer;

        [TestInitialize]
        public void TestInitialize()
        {
            _logicHandler = Substitute.For<ILogicHandler>();
            _logicHandler.GameController = Substitute.For<IGameController>();

            _gameEngineInterface = Substitute.For<IGameEngineInterface>();
            _dataLayer = Substitute.For<IDataLayer>();

            _chooseLevelLogicProvider = new ChooseLevelLogicProvider(_logicHandler, _gameEngineInterface, _dataLayer);
        }

        [TestMethod]
        public void OnStart_FirstLevel()
        {
            #region arrange
            _logicHandler.GameController.CurLevel = new LevelInfo(1, 1);
            _dataLayer.GetCurLevel().Returns(new LevelInfo(1, 1));

            var btnStart = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnChooseLevelStart").Returns(btnStart);

            var btnCancel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnChooseLevelCancel").Returns(btnCancel);

            var btnNextLevel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnNextLevel").Returns(btnNextLevel);

            var btnPrevLevel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnPrevLevel").Returns(btnPrevLevel);
            #endregion

            #region act
            _chooseLevelLogicProvider.OnStart();
            #endregion

            #region
            _dataLayer.Received(1).GetCurLevel();

            _gameEngineInterface.Received(1).FindGameObject("txtSelectedLevel");
            _gameEngineInterface.Received(1).FindGameObject("btnChooseLevelStart");
            _gameEngineInterface.Received(1).FindGameObject("btnChooseLevelCancel");
            _gameEngineInterface.Received(1).FindGameObject("btnNextLevel");
            _gameEngineInterface.Received(1).FindGameObject("btnPrevLevel");
            _gameEngineInterface.Received(1).FindGameObject("pnlChooseLevel");

            Assert.IsNotNull(btnStart.LogicHandler);
            Assert.IsNotNull(btnCancel.LogicHandler);
            Assert.IsNotNull(btnNextLevel.LogicHandler);
            Assert.IsNotNull(btnPrevLevel.LogicHandler);

            btnPrevLevel.Received(1).SetActive(false);
            btnNextLevel.Received(1).SetActive(false);

            var selectedLevelIndex = GetPrivateMember<int>("_selectedLevelIndex", _chooseLevelLogicProvider);
            Assert.AreEqual(0, selectedLevelIndex);

            var maxLevelIndex = GetPrivateMember<int>("_maxLevelIndex", _chooseLevelLogicProvider);
            Assert.AreEqual(selectedLevelIndex, maxLevelIndex);

            var txtSelectedLevel = GetPrivateMember<IText>("_txtSelectedLevel", _chooseLevelLogicProvider);
            Assert.AreEqual("1-1", txtSelectedLevel.Text);
            #endregion
        }

        [TestMethod]
        public void OnStart_NotFirstLevel()
        {
            #region arrange
            _logicHandler.GameController.CurLevel = new LevelInfo(1, 2);
            _dataLayer.GetCurLevel().Returns(new LevelInfo(1, 2));

            var btnStart = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnChooseLevelStart").Returns(btnStart);

            var btnCancel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnChooseLevelCancel").Returns(btnCancel);

            var btnNextLevel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnNextLevel").Returns(btnNextLevel);

            var btnPrevLevel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnPrevLevel").Returns(btnPrevLevel);
            #endregion

            #region act
            _chooseLevelLogicProvider.OnStart();
            #endregion

            #region
            _dataLayer.Received(1).GetCurLevel();

            _gameEngineInterface.Received(1).FindGameObject("txtSelectedLevel");
            _gameEngineInterface.Received(1).FindGameObject("btnChooseLevelStart");
            _gameEngineInterface.Received(1).FindGameObject("btnChooseLevelCancel");
            _gameEngineInterface.Received(1).FindGameObject("btnNextLevel");
            _gameEngineInterface.Received(1).FindGameObject("btnPrevLevel");
            _gameEngineInterface.Received(1).FindGameObject("pnlChooseLevel");

            Assert.IsNotNull(btnStart.LogicHandler);
            Assert.IsNotNull(btnNextLevel.LogicHandler);
            Assert.IsNotNull(btnPrevLevel.LogicHandler);
            Assert.IsNotNull(btnCancel.LogicHandler);

            btnPrevLevel.DidNotReceive().SetActive(false);
            btnNextLevel.Received(1).SetActive(false);

            var selectedLevelIndex = GetPrivateMember<int>("_selectedLevelIndex", _chooseLevelLogicProvider);
            Assert.AreEqual(1, selectedLevelIndex);

            var maxLevelIndex = GetPrivateMember<int>("_maxLevelIndex", _chooseLevelLogicProvider);
            Assert.AreEqual(selectedLevelIndex, maxLevelIndex);

            var txtSelectedLevel = GetPrivateMember<IText>("_txtSelectedLevel", _chooseLevelLogicProvider);
            Assert.AreEqual("1-2", txtSelectedLevel.Text);
            #endregion
        }

        ///This is the case of the player selecting an easier level, then going back to the main menu and starting a new game.
        ///In this case
        ///     * GameController.CurrentLevel is 1 higher than the level the player just beat
        ///     * DataLayer.GetCurrentLevel() returns the 1+ highest level the player has beaten
        ///     * GameController.CurrentLevel may not equal DataLayer.GetCurrentLevel()
        ///     * DataLayer.GetCurrentLevel() should always be the value for the default level and the max level the player can select
        [TestMethod]
        public void OnStart_NotLatestLevel() 
        {
            #region arrange
            _logicHandler.GameController.CurLevel = new LevelInfo(1, 2);
            _dataLayer.GetCurLevel().Returns(new LevelInfo(3, 1));

            var btnStart = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnChooseLevelStart").Returns(btnStart);

            var btnCancel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnChooseLevelCancel").Returns(btnCancel);

            var btnNextLevel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnNextLevel").Returns(btnNextLevel);

            var btnPrevLevel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnPrevLevel").Returns(btnPrevLevel);
            #endregion

            #region actRT
            _chooseLevelLogicProvider.OnStart();
            #endregion

            #region
            var temp = _dataLayer.Received(1).GetCurLevel();

            _gameEngineInterface.Received(1).FindGameObject("txtSelectedLevel");
            _gameEngineInterface.Received(1).FindGameObject("btnChooseLevelStart");
            _gameEngineInterface.Received(1).FindGameObject("btnChooseLevelCancel");
            _gameEngineInterface.Received(1).FindGameObject("btnNextLevel");
            _gameEngineInterface.Received(1).FindGameObject("btnPrevLevel");
            _gameEngineInterface.Received(1).FindGameObject("pnlChooseLevel");

            Assert.IsNotNull(btnStart.LogicHandler);
            Assert.IsNotNull(btnNextLevel.LogicHandler);
            Assert.IsNotNull(btnPrevLevel.LogicHandler);
            Assert.IsNotNull(btnCancel.LogicHandler);

            btnPrevLevel.DidNotReceive().SetActive(false);
            btnNextLevel.Received(1).SetActive(false);

            var selectedLevelIndex = GetPrivateMember<int>("_selectedLevelIndex", _chooseLevelLogicProvider);
            Assert.AreEqual(6, selectedLevelIndex);

            var maxLevelIndex = GetPrivateMember<int>("_maxLevelIndex", _chooseLevelLogicProvider);
            Assert.AreEqual(selectedLevelIndex, maxLevelIndex);

            var txtSelectedLevel = GetPrivateMember<IText>("_txtSelectedLevel", _chooseLevelLogicProvider);
            Assert.AreEqual("3-1", txtSelectedLevel.Text);
            #endregion
        }

        [TestMethod]
        public void findLevelIndex()
        {
            //first level
            var levelInfo = new LevelInfo(1, 1);
            var result = CallPrivateMethod<int>(_chooseLevelLogicProvider, "findLevelIndex", new object[] { levelInfo });
            Assert.AreEqual(0, result);

            //middle level
            levelInfo = new LevelInfo(2, 2);
            result = CallPrivateMethod<int>(_chooseLevelLogicProvider, "findLevelIndex", new object[] { levelInfo });
            Assert.AreEqual(4, result);

            //last level:
            levelInfo = new LevelInfo(5, 3);
            result = CallPrivateMethod<int>(_chooseLevelLogicProvider, "findLevelIndex", new object[] { levelInfo });
            Assert.AreEqual(14, result);

            //error case: no such level
            levelInfo = new LevelInfo(5, 4);
            result = CallPrivateMethod<int>(_chooseLevelLogicProvider, "findLevelIndex", new object[] { levelInfo });
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void OnActivate()
        {
            var pnlChooseLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlChooseLevel", _chooseLevelLogicProvider, pnlChooseLevel);

            _chooseLevelLogicProvider.OnActivate();

            pnlChooseLevel.Received().SetActive(true);
        }

        [TestMethod]
        public void OnDeActivate()
        {
            var pnlChooseLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlChooseLevel", _chooseLevelLogicProvider, pnlChooseLevel);

            _chooseLevelLogicProvider.OnDeActivate();

            pnlChooseLevel.Received().SetActive(false);
        }

        [TestMethod]
        public void btnNextLevel_Click_BaseCase()
        {
            #region arrange
            var txtSelectedLevel = Substitute.For<IText>();
            SetPrivateMember("_txtSelectedLevel", _chooseLevelLogicProvider, txtSelectedLevel);

            var btnPrevLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnPrevLevel", _chooseLevelLogicProvider, btnPrevLevel);

            var btnNextLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnNextLevel", _chooseLevelLogicProvider, btnNextLevel);

            SetPrivateMember("_maxLevelIndex", _chooseLevelLogicProvider, 100);
            #endregion

            #region act
            CallPrivateMethod(_chooseLevelLogicProvider, "btnNextLevel_Click", new object[] { });
            #endregion

            #region assert
            Assert.AreEqual("1-2", txtSelectedLevel.Text);
            btnPrevLevel.Received().SetActive(true);
            btnNextLevel.DidNotReceive().SetActive(Arg.Any<bool>());
            #endregion
        }

        [TestMethod]
        public void btnNextLevel_Click_MaxLevelIndex()
        {
            #region arrange
            var txtSelectedLevel = Substitute.For<IText>();
            SetPrivateMember("_txtSelectedLevel", _chooseLevelLogicProvider, txtSelectedLevel);

            var btnPrevLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnPrevLevel", _chooseLevelLogicProvider, btnPrevLevel);

            var btnNextLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnNextLevel", _chooseLevelLogicProvider, btnNextLevel);

            SetPrivateMember("_maxLevelIndex", _chooseLevelLogicProvider, 1);
            #endregion

            #region act
            CallPrivateMethod(_chooseLevelLogicProvider, "btnNextLevel_Click", new object[] { });
            #endregion

            #region assert
            Assert.AreEqual("1-2", txtSelectedLevel.Text);
            btnPrevLevel.Received().SetActive(true);
            btnNextLevel.Received().SetActive(false);
            #endregion
        }

        [TestMethod]
        public void btnPrevLevel_Click_BaseCase()
        {
            #region arrange
            SetPrivateMember("_selectedLevelIndex", _chooseLevelLogicProvider, 2);

            var txtSelectedLevel = Substitute.For<IText>();
            SetPrivateMember("_txtSelectedLevel", _chooseLevelLogicProvider, txtSelectedLevel);

            var btnPrevLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnPrevLevel", _chooseLevelLogicProvider, btnPrevLevel);

            var btnNextLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnNextLevel", _chooseLevelLogicProvider, btnNextLevel);
            #endregion

            #region act
            CallPrivateMethod(_chooseLevelLogicProvider, "btnPrevLevel_Click", new object[] { });
            #endregion

            #region assert
            Assert.AreEqual("1-2", txtSelectedLevel.Text);
            btnNextLevel.Received().SetActive(true);
            btnPrevLevel.DidNotReceive().SetActive(Arg.Any<bool>());
            #endregion
        }

        [TestMethod]
        public void btnPrevLevel_Click_MinLevelIndex()
        {
            #region arrange
            SetPrivateMember("_selectedLevelIndex", _chooseLevelLogicProvider, 1);

            var txtSelectedLevel = Substitute.For<IText>();
            SetPrivateMember("_txtSelectedLevel", _chooseLevelLogicProvider, txtSelectedLevel);

            var btnPrevLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnPrevLevel", _chooseLevelLogicProvider, btnPrevLevel);

            var btnNextLevel = Substitute.For<IGameObject>();
            SetPrivateMember("_btnNextLevel", _chooseLevelLogicProvider, btnNextLevel);
            #endregion

            #region act
            CallPrivateMethod(_chooseLevelLogicProvider, "btnPrevLevel_Click", new object[] { });
            #endregion

            #region assert
            Assert.AreEqual("1-1", txtSelectedLevel.Text);
            btnNextLevel.Received().SetActive(true);
            btnPrevLevel.Received().SetActive(false);
            #endregion
        }

        [TestMethod]
        public void btnChooseLevelStart_Click()
        {
            _logicHandler.GameController.CurLevel = new LevelInfo(0, 0);

            CallPrivateMethod(_chooseLevelLogicProvider, "btnChooseLevelStart_Click", new object[] { });

            Assert.AreEqual(1, _logicHandler.GameController.CurLevel.Level);
            Assert.AreEqual(1, _logicHandler.GameController.CurLevel.SubLevel);

            _logicHandler.Received().SetSceneState((int)MenuState.Loading);
        }

        [TestMethod]
        public void OnClick_Cancel()
        {
            _chooseLevelLogicProvider.OnClick("btnChooseLevelCancel");

            _logicHandler.Received(1).SetSceneState((int)MenuState.InMenu);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.StoreLogicService;

namespace GameLogicTest
{
    [TestClass]
    public class GameControllerTest : TestBase
    {
        //constructor not testable since it's a singleton

        private GameController _gameController;

        [TestInitialize]
        public void TestInitialize()
        {
            //todo: need to be able to clear all properties (can't  re-instantiate since it's singleton)
            //      * maybe re-evaluate to see if this really needs to be singleton
        }

        [TestMethod]
        public void OnStart_CurLevelNull()
        {
            #region arrange
            var gameController = GameController.Instance;

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();
            gameEngineInterface.AppStoreService.OnPurchaseSucceededEventHandler = null;

            var storeLogicService = Substitute.For<IStoreLogicService>();
            storeLogicService.DataLayer = null;

            var dataLayer = Substitute.For<IDataLayer>();
            #endregion

            #region act
            gameController.OnStart(gameEngineInterface, dataLayer, storeLogicService);
            #endregion

            #region assert
            Assert.AreEqual(gameController, gameController.MenuLogicHandler.GameController);
            Assert.AreEqual(gameController, gameController.GameLogicHandler.GameController);

            Assert.AreEqual(gameEngineInterface, gameController.MenuLogicHandler.GameEngineInterface);
            Assert.AreEqual(gameEngineInterface, gameController.GameLogicHandler.GameEngineInterface);

            Assert.IsNotNull(storeLogicService.DataLayer);
            Assert.IsNotNull(gameEngineInterface.AppStoreService.OnPurchaseSucceededEventHandler);

            dataLayer.Received(1).GetCurLevel(); //this is the case in which the player has not chosen a level yet, so we default it to the latest level they've beaten.
            #endregion
        }

        [TestMethod]
        public void OnStart_CurLevelNotNull()
        {
            #region arrange
            var gameController = GameController.Instance;

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();
            gameEngineInterface.AppStoreService.OnPurchaseSucceededEventHandler = null;

            var storeLogicService = Substitute.For<IStoreLogicService>();
            storeLogicService.DataLayer = null;

            var dataLayer = Substitute.For<IDataLayer>();

            gameController.CurLevel = new LevelInfo(3, 1);
            #endregion

            #region act
            gameController.OnStart(gameEngineInterface, dataLayer, storeLogicService);
            #endregion

            #region assert
            Assert.AreEqual(gameController, gameController.MenuLogicHandler.GameController);
            Assert.AreEqual(gameController, gameController.GameLogicHandler.GameController);

            Assert.AreEqual(gameEngineInterface, gameController.MenuLogicHandler.GameEngineInterface);
            Assert.AreEqual(gameEngineInterface, gameController.GameLogicHandler.GameEngineInterface);

            Assert.IsNotNull(storeLogicService.DataLayer);
            Assert.IsNotNull(gameEngineInterface.AppStoreService.OnPurchaseSucceededEventHandler);

            dataLayer.DidNotReceive().GetCurLevel(); //in this test case, the user has already selected a level on the Choose Menu screen, so we should NOT overwrite the chosen level with the latest level (loaded from disk)
            #endregion
        }

        [TestMethod]
        public void OnStart_SetOnAppStoreInitialized()
        {
            //setup
            //todo: dup mocking code. move common code to TestInitialize()
            var gameController = GameController.Instance;

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();
            gameEngineInterface.AppStoreService.OnPurchaseSucceededEventHandler = null;
            gameEngineInterface.AppStoreService.OnAppStoreInitialized = null;

            var storeLogicService = Substitute.For<IStoreLogicService>();
            storeLogicService.DataLayer = null;

            var dataLayer = Substitute.For<IDataLayer>();
            gameController.CurLevel = new LevelInfo(3, 1);

            //run
            gameController.OnStart(gameEngineInterface, dataLayer, storeLogicService);

            //assert
            Assert.IsNotNull(gameEngineInterface.AppStoreService.OnAppStoreInitialized);
        }

        [TestMethod]
        public void OnUpdate_ShouldUpdate()
        {
            #region arrange
            var gameController = GameController.Instance;
            gameController.ShouldUpdate = true;

            var curLogicHandler = Substitute.For<ILogicHandler>();
            SetPrivateMember("_curLogicHandler", gameController, curLogicHandler);
            #endregion

            #region act
            gameController.OnUpdate();
            #endregion

            #region assert
            curLogicHandler.Received().OnUpdate();
            #endregion
        }

        [TestMethod]
        public void OnUpdate_ShouldNotUpdate()
        {
            #region arrange
            var gameController = GameController.Instance;
            gameController.ShouldUpdate = false;

            var curLogicHandler = Substitute.For<ILogicHandler>();
            SetPrivateMember("_curLogicHandler", gameController, curLogicHandler);
            #endregion

            #region act
            gameController.OnUpdate();
            #endregion

            #region assert
            curLogicHandler.DidNotReceive().OnUpdate();
            #endregion
        }

        [TestMethod]
        public void SetAppState()
        {
            #region arrange
            var gameController = GameController.Instance;

            gameController.GameEngineInterface = Substitute.For<IGameEngineInterface>();
            #endregion

            #region act
            gameController.SetAppState(AppState.MainMenu);
            #endregion

            #region assert
            gameController.GameEngineInterface.Received().LoadScene(Constants.SceneNames.UIScene);
            Assert.IsFalse(gameController.ShouldUpdate);
            #endregion
        }
        
        [TestMethod]
        public void LoadSceneAsync()
        {
            #region arrange
            var gameController = GameController.Instance;

            gameController.GameEngineInterface = Substitute.For<IGameEngineInterface>();
            #endregion

            #region act
            gameController.LoadSceneAsync("");
            #endregion

            #region assert
            gameController.GameEngineInterface.Received().LoadSceneAsync(Constants.SceneNames.GameScene);
            Assert.IsFalse(gameController.JustBeatMaxLevel); //we reset this to false here b/c you can't have just beaten the max level if you're loading a new level
            #endregion
        }

        [TestMethod]
        public void ProgressToNextLevel_SubLevel()
        {
            #region arrange
            var gameController = GameController.Instance;

            gameController.GameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameController.CurLevel = new LevelInfo(1, Constants.Levels.NumSubLevels-1);
            gameController.DataLayer = Substitute.For<IDataLayer>();
            gameController.DataLayer.GetCurLevel().Returns(new LevelInfo(1, Constants.Levels.NumSubLevels - 1));
            #endregion

            #region act
            gameController.ProgressToNextLevel();
            #endregion

            #region assert
            Assert.AreEqual(1, gameController.CurLevel.Level);
            Assert.AreEqual(Constants.Levels.NumSubLevels, gameController.CurLevel.SubLevel);

            Assert.IsFalse(gameController.JustBeatMaxLevel);

            gameController.DataLayer.Received().SaveCurLevel(gameController.CurLevel);
            Assert.IsTrue(gameController.JustSavedLatestLevel);
            #endregion
        }

        [TestMethod]
        public void ProgressToNextLevel_MainLevel()
        {
            #region arrange
            var gameController = GameController.Instance;

            gameController.GameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameController.CurLevel = new LevelInfo(1, Constants.Levels.NumSubLevels);
            gameController.DataLayer = Substitute.For<IDataLayer>();
            gameController.DataLayer.GetCurLevel().Returns(new LevelInfo(1, Constants.Levels.NumLevels));
            #endregion

            #region act
            gameController.ProgressToNextLevel();
            #endregion

            #region assert
            Assert.AreEqual(2, gameController.CurLevel.Level);
            Assert.AreEqual(1, gameController.CurLevel.SubLevel);

            Assert.IsFalse(gameController.JustBeatMaxLevel);

            gameController.DataLayer.Received().SaveCurLevel(gameController.CurLevel);
            Assert.IsTrue(gameController.JustSavedLatestLevel);
            #endregion
        }

        [TestMethod]
        public void ProgressToNextLevel_BeatGame()
        {
            #region arrange
            var gameController = GameController.Instance;

            gameController.GameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameController.CurLevel = new LevelInfo(Constants.Levels.NumLevels, Constants.Levels.NumSubLevels);
            gameController.DataLayer = Substitute.For<IDataLayer>();
            gameController.DataLayer.GetCurLevel().Returns(new LevelInfo(5, 3));
            #endregion

            #region act
            gameController.ProgressToNextLevel();
            #endregion

            #region assert
            Assert.AreEqual(Constants.Levels.NumLevels, gameController.CurLevel.Level);
            Assert.AreEqual(Constants.Levels.NumSubLevels, gameController.CurLevel.SubLevel);

            Assert.IsTrue(gameController.JustBeatMaxLevel);

            gameController.DataLayer.DidNotReceive().SaveCurLevel(gameController.CurLevel); //there is no level higher than the max level, gameController.CurrentLevel isn't greater than _dataLayer.GetCurLevel
            Assert.IsFalse(gameController.JustSavedLatestLevel);
            #endregion
        }

        /// <summary>
        /// if the player selectes an earlier level on the Choose Level screen, then when they beat the level, we shouldn't save their progress.
        /// for example, if their latest level is 3-2, but they went back to play 1-3 just for fun, then we don't want to save 2-1 as their latest level, because 2-1 is lower than 3-2.
        /// </summary>
        [TestMethod]
        public void ProgressToNextLevel_NotLatestLevel()
        {
            #region arrange
            var gameController = GameController.Instance;

            gameController.GameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameController.CurLevel = new LevelInfo(1, 1);
            gameController.DataLayer = Substitute.For<IDataLayer>();
            gameController.DataLayer.GetCurLevel().Returns(new LevelInfo(4, 1));
            #endregion

            #region act
            gameController.ProgressToNextLevel();
            #endregion

            #region assert
            Assert.AreEqual(1, gameController.CurLevel.Level);
            Assert.AreEqual(2, gameController.CurLevel.SubLevel);

            Assert.IsFalse(gameController.JustBeatMaxLevel);

            gameController.DataLayer.DidNotReceive().SaveCurLevel(gameController.CurLevel);
            Assert.IsFalse(gameController.JustSavedLatestLevel);
            #endregion
        }

    }
}

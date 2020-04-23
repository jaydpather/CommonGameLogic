using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders;

namespace GameLogicTest.LogicHandlers
{
    [TestClass]
    public class GameLogicHandlerTest : TestBase
    {
        [TestMethod]
        public void OnStart()
        {
            #region Arrange
            var gameLogicHandler = GameLogicHandler.Instance;

            gameLogicHandler.GameController = Substitute.For<IGameController>();
            gameLogicHandler.GameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameLogicHandler.GameEngineInterface.Advertisement.Returns(Substitute.For<IAdvertisement>());

            gameLogicHandler.GameLogicProvider = Substitute.For<ILogicProvider>();
            gameLogicHandler.PauseLogicProvider = Substitute.For<ILogicProvider>();
            gameLogicHandler.WinTransitionLogicProvider = Substitute.For<ILogicProvider>();
            gameLogicHandler.WinLogicProvider= Substitute.For<ILogicProvider>();
            gameLogicHandler.WinLogicProvider = Substitute.For<ILogicProvider>();
            gameLogicHandler.LoseTransitionLogicProvider = Substitute.For<ILogicProvider>();
            gameLogicHandler.LoseLogicProvider = Substitute.For<ILogicProvider>();
            #endregion

            #region Act
            gameLogicHandler.OnStart();
            #endregion

            #region assert
            Assert.AreEqual(gameLogicHandler.GameLogicProvider, gameLogicHandler.CurLogicProvider);
            Assert.AreEqual(true, gameLogicHandler.GameController.ShouldUpdate);
            gameLogicHandler.GameEngineInterface.Received().ClearGameObjectCache();

            gameLogicHandler.GameLogicProvider.Received().OnStart();
            gameLogicHandler.PauseLogicProvider.Received().OnStart();
            gameLogicHandler.WinTransitionLogicProvider.Received().OnStart();
            gameLogicHandler.WinLogicProvider.Received().OnStart();
            gameLogicHandler.LoseTransitionLogicProvider.Received().OnStart();
            gameLogicHandler.LoseLogicProvider.Received().OnStart();

            Assert.AreEqual(gameLogicHandler.GameLogicProvider, gameLogicHandler.CurLogicProvider);
            #endregion  
        }

        [TestMethod]
        public void OnUpdate()
        {
            #region arrange
            var gameLogicHandler = GameLogicHandler.Instance;
            gameLogicHandler.CurLogicProvider = Substitute.For<ILogicProvider>();
            #endregion

            #region act
            gameLogicHandler.OnUpdate();
            #endregion

            #region assert
            gameLogicHandler.CurLogicProvider.Received().UpdateInputStates();
            gameLogicHandler.CurLogicProvider.Received().HandleInput();
            gameLogicHandler.CurLogicProvider.Received().ClearInputStates();
            gameLogicHandler.CurLogicProvider.Received().UpdateGameObjects();
            #endregion
        }

        [TestMethod]
        public void OnClick()
        {
            #region arrange
            var gameLogicHandler = GameLogicHandler.Instance;
            gameLogicHandler.CurLogicProvider = Substitute.For<ILogicProvider>();
            #endregion

            #region act
            gameLogicHandler.OnClick("sender");
            #endregion

            #region assert
            gameLogicHandler.CurLogicProvider.Received().OnClick("sender");
            #endregion
        }

        [TestMethod]
        public void OnCollision()
        {
            #region arrange
            var gameLogicHandler = GameLogicHandler.Instance;
            gameLogicHandler.CurLogicProvider = Substitute.For<ILogicProvider>();
            #endregion

            #region act
            gameLogicHandler.OnCollision(null, null);
            #endregion

            #region assert
            gameLogicHandler.CurLogicProvider.Received().OnCollision(null, null);
            #endregion
        }

        [TestMethod]
        public void SetSceneState()
        {
            #region arrange
            var gameLogicHandler = GameLogicHandler.Instance;
            var origCurLogicProvider = Substitute.For<ILogicProvider>();
            gameLogicHandler.CurLogicProvider = origCurLogicProvider;
            gameLogicHandler.PauseLogicProvider = Substitute.For<ILogicProvider>();

            var logicProvidersByGameState = GetPrivateMember<Dictionary<GameState, ILogicProvider>>("_logicProvidersByGameState", gameLogicHandler);
            logicProvidersByGameState.Clear();
            logicProvidersByGameState.Add(GameState.Pause, gameLogicHandler.PauseLogicProvider);
            #endregion

            #region act
            gameLogicHandler.SetSceneState((int)GameState.Pause);
            #endregion

            #region assert
            origCurLogicProvider.Received().ClearInputStates();
            origCurLogicProvider.Received().OnDeActivate();

            Assert.AreEqual(gameLogicHandler.PauseLogicProvider, gameLogicHandler.CurLogicProvider);
            gameLogicHandler.PauseLogicProvider.Received().OnActivate();
            #endregion

            #region clean up
            logicProvidersByGameState.Clear();
            #endregion
        }

        [TestMethod]
        public void SetAppState()
        {
            #region arrange
            var gameLogicHandler = GameLogicHandler.Instance;
            gameLogicHandler.GameController = Substitute.For<IGameController>();
            #endregion

            #region act
            gameLogicHandler.SetAppState(AppState.MainMenu);
            #endregion

            #region assert
            gameLogicHandler.GameController.Received().SetAppState(AppState.MainMenu);
            #endregion
        }
    }
}

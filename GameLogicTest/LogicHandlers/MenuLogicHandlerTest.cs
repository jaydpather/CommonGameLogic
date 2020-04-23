using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders;

namespace GameLogicTest.LogicHandlers
{
    [TestClass]
    public class MenuLogicHandlerTest : TestBase
    {
        [TestMethod]
        public void LogToDebugOutput()
        {
            #region arrange
            var menuLogicHandler = MenuLogicHandler.Instance;
            menuLogicHandler.CurLogicProvider = Substitute.For<ILogicProvider>();
            #endregion

            #region act
            CallPrivateMethod(menuLogicHandler, "LogToDebugOutput", new object[] { "my message" });
            #endregion

            #region assert
            menuLogicHandler.CurLogicProvider.Received().LogToDebugOutput("my message");
            #endregion
        }

        [TestMethod]
        public void OnClick()
        {
            #region arrange
            var menuLogicHandler = MenuLogicHandler.Instance;
            menuLogicHandler.CurLogicProvider = Substitute.For<ILogicProvider>();
            #endregion

            #region act
            menuLogicHandler.OnClick("sender");
            #endregion

            #region assert
            menuLogicHandler.CurLogicProvider.Received().OnClick("sender");
            #endregion
        }

        [TestMethod]
        public void OnStart()
        {
            #region arrange
            var menuLogicHandler = MenuLogicHandler.Instance;
            menuLogicHandler.GameController = Substitute.For<IGameController>();
            menuLogicHandler.LoadingLogicProvider = Substitute.For<ILogicProvider>();
            menuLogicHandler.InMenuLogicProvider = Substitute.For<ILogicProvider>();
            menuLogicHandler.GameEngineInterface = Substitute.For<IGameEngineInterface>();
            menuLogicHandler.ChooseLevelLogicProvider = Substitute.For<ILogicProvider>();
            menuLogicHandler.HowToPlayLogicProvider = Substitute.For<ILogicProvider>();
            menuLogicHandler.GameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();
            menuLogicHandler.DefaultSceneState = (int)MenuState.HowToPlay; //this lets us check if it reset to the default value
            #endregion

            #region act
            menuLogicHandler.OnStart();
            #endregion

            #region assert
            Assert.IsTrue(menuLogicHandler.GameController.ShouldUpdate);
            Assert.AreEqual(menuLogicHandler.HowToPlayLogicProvider, menuLogicHandler.CurLogicProvider); //this is based on DefaultSceneState
            menuLogicHandler.InMenuLogicProvider.Received().OnStart();
            menuLogicHandler.LoadingLogicProvider.Received().OnStart();
            menuLogicHandler.ChooseLevelLogicProvider.Received().OnStart();
            menuLogicHandler.HowToPlayLogicProvider.Received(1).OnStart();
            //todo: UT does not check that we called GetMoreLivesLogicProvider.OnStart();

            menuLogicHandler.HowToPlayLogicProvider.Received(1).OnActivate(); //this checks that we called SetSceneState()

            Assert.AreEqual((int)MenuState.InMenu, menuLogicHandler.DefaultSceneState); //check that we reset to the default value

            Assert.IsNotNull(menuLogicHandler.GameEngineInterface.AppStoreService.LogToDebugOutput);
            #endregion
        }

        [TestMethod]
        public void OnUpdate()
        {
            #region arrange
            var menuLogicHandler = MenuLogicHandler.Instance;
            menuLogicHandler.CurLogicProvider = Substitute.For<ILogicProvider>();
            #endregion

            #region act
            menuLogicHandler.OnUpdate();
            #endregion

            #region assert
            menuLogicHandler.CurLogicProvider.Received().UpdateInputStates();
            menuLogicHandler.CurLogicProvider.Received().HandleInput();
            menuLogicHandler.CurLogicProvider.Received().UpdateGameObjects();
            #endregion
        }

        [TestMethod]
        public void SetAppState()
        {
            #region arrange
            var menuLogicHandler = MenuLogicHandler.Instance;
            menuLogicHandler.GameController = Substitute.For<IGameController>();
            #endregion

            #region act
            menuLogicHandler.SetAppState(AppState.Game);
            #endregion

            #region assert
            menuLogicHandler.GameController.Received().SetAppState(AppState.Game);
            #endregion
        }

        [TestMethod]
        public void SetSceneState()
        {
            #region arrange
            var menuLogicHandler = MenuLogicHandler.Instance;
            var origCurLogicProvider = Substitute.For<ILogicProvider>();
            menuLogicHandler.CurLogicProvider = origCurLogicProvider;
            menuLogicHandler.LoadingLogicProvider = Substitute.For<ILogicProvider>();

            var logicProvidersByGameState = GetPrivateMember<Dictionary<MenuState, ILogicProvider>>("_logicProvidersByState", menuLogicHandler);
            logicProvidersByGameState.Clear();
            logicProvidersByGameState.Add(MenuState.Loading, menuLogicHandler.LoadingLogicProvider);
            #endregion

            #region act
            menuLogicHandler.SetSceneState((int)MenuState.Loading);
            #endregion

            #region assert
            origCurLogicProvider.Received().ClearInputStates();
            origCurLogicProvider.Received().OnDeActivate();

            Assert.AreEqual(menuLogicHandler.LoadingLogicProvider, menuLogicHandler.CurLogicProvider);
            menuLogicHandler.LoadingLogicProvider.Received().OnActivate();
            #endregion

            #region clean up
            logicProvidersByGameState.Clear();
            #endregion
        }

        [TestMethod]
        public void SetSceneState_OrigLogicProviderNull() //todo v2: seems like we don't need this test. I don't think you can ever hit the case of menuLogicHandler._curLogicHandler == null when running the app for real
        {
            #region arrange
            var menuLogicHandler = MenuLogicHandler.Instance;
            menuLogicHandler.CurLogicProvider = null;
            menuLogicHandler.LoadingLogicProvider = Substitute.For<ILogicProvider>();

            var logicProvidersByGameState = GetPrivateMember<Dictionary<MenuState, ILogicProvider>>("_logicProvidersByState", menuLogicHandler);
            logicProvidersByGameState.Clear();
            logicProvidersByGameState.Add(MenuState.Loading, menuLogicHandler.LoadingLogicProvider);
            #endregion

            #region act
            var caughtException = false; //original value of menuLogicHandler.CurLogicProvider is null, so we could get an exception
            try
            {
                menuLogicHandler.SetSceneState((int)MenuState.Loading);
            }
            catch
            {
                caughtException = true;
            }
            #endregion

            #region assert
            Assert.IsFalse(caughtException);

            Assert.AreEqual(menuLogicHandler.LoadingLogicProvider, menuLogicHandler.CurLogicProvider);
            menuLogicHandler.LoadingLogicProvider.Received().OnActivate();
            #endregion

            #region clean up
            logicProvidersByGameState.Clear();
            #endregion
        }
    }
}

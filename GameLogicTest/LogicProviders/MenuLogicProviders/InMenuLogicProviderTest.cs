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
using static ThirdEyeSoftware.GameLogic.LogicProviders.BaseLogicProvider;

namespace GameLogicTest.LogicProviders.MenuLogicProviders
{
    [TestClass]
    public class InMenuLogicProviderTest : TestBase
    {
        private ILogicHandler _logicHandler;
        private IGameEngineInterface _gameEngineInterface;
        private IDataLayer _dataLayer;

        private InMenuLogicProvider _inMenuLogicProvider;
        private IGameObject _btnPrivacyPolicy;
        private IGameObject _btnHowToPlay;
        private IGameObject _btnShare;
        private IGameObject _pnlMainMenu;
        private IText _txtVersionNumber;

        [TestInitialize]
        public void TestInitialize()
        {
            _logicHandler = Substitute.For<ILogicHandler>();
            _gameEngineInterface = Substitute.For<IGameEngineInterface>();
            _dataLayer = Substitute.For<IDataLayer>();

            _gameEngineInterface.AppVersion.Returns("1.0.0.0");

            _btnPrivacyPolicy = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnPrivacyPolicy").Returns(_btnPrivacyPolicy);

            _btnHowToPlay = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnHowToPlay").Returns(_btnHowToPlay);

            _btnShare = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("_btnShare").Returns(_btnShare); //todo: howcome the UT works with a misnamed button?

            var txtVersionNumberGameObject = Substitute.For<IGameObject>();
            _txtVersionNumber = Substitute.For<IText>();
            txtVersionNumberGameObject.GetComponent<IText>().Returns(_txtVersionNumber);
            _gameEngineInterface.FindGameObject("txtVersionNumber").Returns(txtVersionNumberGameObject);
            
            _pnlMainMenu = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("pnlMainMenu").Returns(_pnlMainMenu);

            _inMenuLogicProvider = new InMenuLogicProvider(_logicHandler, _gameEngineInterface, _dataLayer);
        }

        [TestMethod]
        public void OnStart()
        {
            #region arrange
            _gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();
            _gameEngineInterface.AppStoreService.OnPurchaseFailedEventHandler = null;

            var btnStartGame = Substitute.For<IGameObject>();
            btnStartGame.LogicHandler = null;
            _gameEngineInterface.FindGameObject("btnStartGame").Returns(btnStartGame);

            var btnGetMoreLives = Substitute.For<IGameObject>();
            btnGetMoreLives.LogicHandler = null;
            _gameEngineInterface.FindGameObject("btnGetMoreLives").Returns(btnGetMoreLives);

            var btnExitGame = Substitute.For<IGameObject>();
            btnExitGame.LogicHandler = null;
            _gameEngineInterface.FindGameObject("btnExitGame").Returns(btnExitGame);

            var btnPurchaseSucceededOk = Substitute.For<IGameObject>();
            btnPurchaseSucceededOk.LogicHandler = null;
            _gameEngineInterface.FindGameObject("btnStorePurchaseSucceededOk").Returns(btnPurchaseSucceededOk);

            var btnPurchaseFailedOk = Substitute.For<IGameObject>();
            btnPurchaseFailedOk.LogicHandler = null;
            _gameEngineInterface.FindGameObject("btnStorePurchaseFailedOk").Returns(btnPurchaseFailedOk);

            var pnlPurchaseSucceeded = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("pnlStorePurchaseSucceeded").Returns(pnlPurchaseSucceeded);

            var pnlPurchaseFailed = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("pnlStorePurchaseFailed").Returns(pnlPurchaseFailed);

            var pnlChooseLevel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("pnlChooseLevel").Returns(pnlChooseLevel);

            #endregion

            #region act
            _inMenuLogicProvider.OnStart();
            #endregion

            #region assert
            _gameEngineInterface.Received(1).FindGameObject("btnStartGame");
            Assert.IsNotNull(btnStartGame.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("btnHowToPlay");
            Assert.IsNotNull(_btnHowToPlay.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("btnShare");
            Assert.IsNotNull(_btnShare.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("btnGetMoreLives");
            Assert.IsNotNull(btnGetMoreLives.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("btnExitGame");
            Assert.IsNotNull(btnExitGame.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("btnPrivacyPolicy");
            Assert.IsNotNull(_btnPrivacyPolicy.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("txtVersionNumber");
            var expectedVersionNumber = "v" + _gameEngineInterface.AppVersion;
            Assert.AreEqual(expectedVersionNumber, _txtVersionNumber.Text);

            _gameEngineInterface.Received(1).FindGameObject("btnStorePurchaseSucceededOk");
            Assert.IsNotNull(btnPurchaseSucceededOk.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("btnStorePurchaseFailedOk");
            Assert.IsNotNull(btnPurchaseFailedOk.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("pnlMainMenu");
            _pnlMainMenu.Received(1).SetActive(false);

            _gameEngineInterface.Received(1).FindGameObject("pnlStorePurchaseSucceeded");
            pnlPurchaseSucceeded.Received(1).SetActive(false);

            _gameEngineInterface.Received(1).FindGameObject("pnlStorePurchaseFailed");
            pnlPurchaseFailed.Received(1).SetActive(false);

            _gameEngineInterface.Received(1).FindGameObject("pnlChooseLevel");
            pnlChooseLevel.Received(1).SetActive(false);

            Assert.AreEqual(1, _gameEngineInterface.TimeScale);

            Assert.IsNotNull(_gameEngineInterface.AppStoreService.OnPurchaseFailedEventHandler);
            #endregion
        }

        [TestMethod]
        public void OnActivate()
        {
            var pnlMainMenu = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlMainMenu", _inMenuLogicProvider, pnlMainMenu);

            _inMenuLogicProvider.OnActivate();

            pnlMainMenu.Received(1).SetActive(true);
        }

        [TestMethod]
        public void OnDeActivate()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            
            var dataLayer = Substitute.For<IDataLayer>();
            
            var inMenuLogicProvider = new InMenuLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer);
            var pnlMainMenu = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlMainMenu", inMenuLogicProvider, pnlMainMenu);
            #endregion

            #region act
            inMenuLogicProvider.OnDeActivate();
            #endregion

            #region assert
            pnlMainMenu.Received().SetActive(false);
            #endregion
        }

        //todo real game: why is this commented out?
        //[TestMethod]
        //public void OnAppStorePurchaseSucceeded()
        //{
        //    #region arrange
        //    var gameLogicHandler = Substitute.For<ILogicHandler>();

        //    var gameEngineInterface = Substitute.For<IGameEngineInterface>();

        //    var dataLayer = Substitute.For<IDataLayer>();

        //    var inMenuLogicProvider = Substitute.ForPartsOf<InMenuLogicProvider>(gameLogicHandler, gameEngineInterface, dataLayer);

        //    var btnRemoveAds = Substitute.For<IGameObject>();
        //    inMenuLogicProvider.btnBuyOneLife = btnRemoveAds;

        //    inMenuLogicProvider.WhenForAnyArgs(x => x.LogToDebugOutput(Arg.Any<string>())).DoNotCallBase();
        //    #endregion

        //    #region act
        //    inMenuLogicProvider.OnAppStorePurchaseSucceeded(Constants.ProductNames.OneLife);
        //    #endregion

        //    #region assert
        //    dataLayer.Received().SetBool(Constants.SavedDataKeys.ShouldDisplayAds, false);
        //    dataLayer.Received().Save();
        //    btnRemoveAds.Received().SetActive(false);
        //    #endregion
        //}

        [TestMethod]
        public void OnClick()
        {
            var menuUIInputStates = GetPrivateMember<Dictionary<MenuUIInputAxis, bool>>("_menuUIInputStates", _inMenuLogicProvider);
            menuUIInputStates[MenuUIInputAxis.btnGetMoreLives] = false;
            menuUIInputStates[MenuUIInputAxis.btnPurchaseFailedOk] = false;
            menuUIInputStates[MenuUIInputAxis.btnPurchaseSucceededOk] = false;
            menuUIInputStates[MenuUIInputAxis.btnStartGame] = false;

            _inMenuLogicProvider.OnClick("btnGetMoreLives");
            Assert.IsTrue(menuUIInputStates[MenuUIInputAxis.btnGetMoreLives]);

            _inMenuLogicProvider.OnClick("btnStorePurchaseFailedOk");
            Assert.IsTrue(menuUIInputStates[MenuUIInputAxis.btnPurchaseFailedOk]);

            _inMenuLogicProvider.OnClick("btnStorePurchaseSucceededOk");
            Assert.IsTrue(menuUIInputStates[MenuUIInputAxis.btnPurchaseSucceededOk]);

            _inMenuLogicProvider.OnClick("btnStartGame");
            Assert.IsTrue(menuUIInputStates[MenuUIInputAxis.btnStartGame]);
        }

        [TestMethod]
        public void OnClick_Share()
        {
            _inMenuLogicProvider.OnClick("btnShare");
            _logicHandler.GameEngineInterface.Received(1).OpenShareDialog(Constants.URLs.PrivacyPolicy);
        }

        [TestMethod]
        public void OnClick_HowToPlay()
        {
            _inMenuLogicProvider.OnClick("btnHowToPlay");

            _logicHandler.Received(1).SetSceneState((int)MenuState.HowToPlay);
        }

        [TestMethod]
        public void OnClick_PrivacyPolicy()
        {
            _inMenuLogicProvider.OnClick("btnPrivacyPolicy");
            _gameEngineInterface.Received(1).OpenURL(Constants.URLs.PrivacyPolicy);
        }

        [TestMethod]
        public void OnAppStorePurchaseFailed()
        {
            #region arrange
            var pnlPurchaseFailed = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlPurchaseFailed", _inMenuLogicProvider, pnlPurchaseFailed);

            var pnlMainMenu = Substitute.For<IGameObject>();
            SetPrivateMember("_pnlMainMenu", _inMenuLogicProvider, pnlMainMenu);
            #endregion

            #region act
            CallPrivateMethod(_inMenuLogicProvider, "OnAppStorePurchaseFailed", new object[0]);
            #endregion

            #region assert
            pnlPurchaseFailed.Received(1).SetActive(true);
            pnlMainMenu.Received(1).SetActive(false);
            #endregion
        }

        [TestMethod]
        public void DoesPlayerHaveLives_HasLives()
        {
            _dataLayer.GetNumLivesRemaining().Returns(1);

            var result = CallPrivateMethod<bool>(_inMenuLogicProvider, "DoesPlayerHaveLives", new object[] { });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DoesPlayerHaveLives_OutOfLives()
        {
            _dataLayer.GetNumLivesRemaining().Returns(0);

            var result = CallPrivateMethod<bool>(_inMenuLogicProvider, "DoesPlayerHaveLives", new object[] { });

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HandleInput_StartGame_HasLives()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            dataLayer.GetNumLivesRemaining().Returns(1);

            var inMenuLogicProvider = Substitute.ForPartsOf<InMenuLogicProvider>(gameLogicHandler, gameEngineInterface, dataLayer);

            var menuUIInputStates = GetPrivateMember<Dictionary<MenuUIInputAxis, bool>>("_menuUIInputStates", inMenuLogicProvider);
            menuUIInputStates[MenuUIInputAxis.btnStartGame] = true;

            var menuLogicHandler = Substitute.For<ILogicHandler>();
            inMenuLogicProvider.MenuLogicHandler = menuLogicHandler;
            #endregion

            #region act
            inMenuLogicProvider.HandleInput();
            #endregion

            #region assert
            menuLogicHandler.Received().SetSceneState((int)MenuState.ChooseLevel);
            #endregion
        }

        [TestMethod]
        public void HandleInput_StartGame_OutOfLives() 
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            dataLayer.GetNumLivesRemaining().Returns(0);

            var inMenuLogicProvider = Substitute.ForPartsOf<InMenuLogicProvider>(gameLogicHandler, gameEngineInterface, dataLayer);

            var menuUIInputStates = GetPrivateMember<Dictionary<MenuUIInputAxis, bool>>("_menuUIInputStates", inMenuLogicProvider);
            menuUIInputStates[MenuUIInputAxis.btnStartGame] = true;

            var menuLogicHandler = Substitute.For<ILogicHandler>();
            inMenuLogicProvider.MenuLogicHandler = menuLogicHandler;
            #endregion

            #region act
            inMenuLogicProvider.HandleInput();
            #endregion

            #region assert
            menuLogicHandler.Received().SetSceneState((int)MenuState.OutOfLives);
            #endregion
        }

        [TestMethod]
        public void HandleInput_GetMoreLives()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();

            gameEngineInterface.AppStoreService = Substitute.For<IAppStoreService>();

            var inMenuLogicProvider = Substitute.ForPartsOf<InMenuLogicProvider>(gameLogicHandler, gameEngineInterface, dataLayer);

            var menuUIInputStates = GetPrivateMember<Dictionary<MenuUIInputAxis, bool>>("_menuUIInputStates", inMenuLogicProvider);
            menuUIInputStates[MenuUIInputAxis.btnStartGame] = false;
            menuUIInputStates[MenuUIInputAxis.btnGetMoreLives] = true;

            var menuLogicHandler = Substitute.For<ILogicHandler>();
            inMenuLogicProvider.MenuLogicHandler = menuLogicHandler;
            #endregion

            #region act
            inMenuLogicProvider.HandleInput();
            #endregion

            #region assert
            Assert.IsFalse(menuUIInputStates[MenuUIInputAxis.btnGetMoreLives]);
            menuLogicHandler.Received().SetSceneState((int)MenuState.GetMoreLives); //todo 2nd game: integration test that confirms that when you call SetSceneState with this param, it actually calls the GetMoreLivesLogicProvider.OnActivate()

            #endregion
        }

        [TestMethod]
        public void btnExitGame_Click()
        {
            _inMenuLogicProvider.btnExitGame_Click();

            _gameEngineInterface.Received(1).ExitApp();
        }
    }
}

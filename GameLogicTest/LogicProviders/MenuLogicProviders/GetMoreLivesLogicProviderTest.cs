using GameLogic.LogicProviders.MenuLogicProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace GameLogicTest.LogicProviders.MenuLogicProviders
{
    [TestClass]
    public class GetMoreLivesLogicProviderTest
    {
        private GetMoreLivesLogicProvider _getMoreLivesLogicProvider;
        private ILogicHandler _logicHandler;
        private IGameEngineInterface _gameEngineInterface;
        private IDataLayer _dataLayer;

        private IGameObject _pnlGetMoreLives; 
        private IGameObject _btnCancel;
        private IGameObject _btnBuyLivesSmall;
        private IGameObject _btnBuyLivesMedium;
        private IGameObject _btnBuyLivesLarge;
        private IGameObject _txtCurrentLivesGameObject;
        private IText _txtCurrentLives;

        [TestInitialize]
        public void TestInitialize()
        {
            _logicHandler = Substitute.For<ILogicHandler>();
            _gameEngineInterface = Substitute.For<IGameEngineInterface>();
            _dataLayer = Substitute.For<IDataLayer>();

            _dataLayer.GetNumLivesRemaining().Returns(10);

            _getMoreLivesLogicProvider = new GetMoreLivesLogicProvider(_logicHandler, _gameEngineInterface, _dataLayer);

            _pnlGetMoreLives = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("pnlGetMoreLives").Returns(_pnlGetMoreLives);

            _btnCancel = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnGetMoreLivesCancel").Returns(_btnCancel);

            _btnBuyLivesSmall = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnBuyLivesSmall").Returns(_btnBuyLivesSmall);

            _btnBuyLivesMedium = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnBuyLivesMedium").Returns(_btnBuyLivesMedium);

            _btnBuyLivesLarge = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnBuyLivesLarge").Returns(_btnBuyLivesLarge);

            _txtCurrentLivesGameObject = Substitute.For<IGameObject>();
            _txtCurrentLives = Substitute.For<IText>();
            _txtCurrentLivesGameObject.GetComponent<IText>().Returns(_txtCurrentLives);
            _gameEngineInterface.FindGameObject("txtCurrentLives").Returns(_txtCurrentLivesGameObject);
        }

        [TestMethod]
        public void OnStart()
        {
            _getMoreLivesLogicProvider.OnStart();

            _gameEngineInterface.Received().FindGameObject("btnGetMoreLivesCancel");
            Assert.AreEqual(_logicHandler, _btnCancel.LogicHandler);

            _gameEngineInterface.Received().FindGameObject("btnBuyLivesSmall");
            Assert.AreEqual(_logicHandler, _btnBuyLivesSmall.LogicHandler);

            _gameEngineInterface.Received().FindGameObject("btnBuyLivesMedium");
            Assert.AreEqual(_logicHandler, _btnBuyLivesMedium.LogicHandler);

            _gameEngineInterface.Received().FindGameObject("btnBuyLivesLarge");
            Assert.AreEqual(_logicHandler, _btnBuyLivesLarge.LogicHandler);

            _gameEngineInterface.Received(1).FindGameObject("txtCurrentLives");
            _txtCurrentLivesGameObject.Received(1).GetComponent<IText>();
            var expectedText = "REMAINING LIVES: " + _dataLayer.GetNumLivesRemaining();
            Assert.AreEqual(_txtCurrentLives.Text, expectedText);

            _gameEngineInterface.Received().FindGameObject("pnlGetMoreLives");
            _pnlGetMoreLives.Received().SetActive(false);
        }

        [TestMethod]
        public void OnActivate()
        {
            //init:
            _getMoreLivesLogicProvider.OnStart();

            //call:
            _getMoreLivesLogicProvider.OnActivate();

            //verify:
            _pnlGetMoreLives.Received().SetActive(true);
        }

        [TestMethod]
        public void OnClick_Cancel()
        {
            _getMoreLivesLogicProvider.OnClick("btnGetMoreLivesCancel");
            _logicHandler.Received().SetSceneState((int)MenuState.InMenu);
        }

        [TestMethod]
        public void OnDeactivate()
        {
            //init:
            _getMoreLivesLogicProvider.OnStart();

            _getMoreLivesLogicProvider.OnDeActivate();

            _pnlGetMoreLives.Received().SetActive(false);
        }

        [TestMethod]
        public void OnClick_btnBuyLivesSmall()
        {
            _getMoreLivesLogicProvider.OnClick("btnBuyLivesSmall");
            _gameEngineInterface.AppStoreService.Received(1).BuyProductByID(Constants.ProductNames.BuyLivesSmall);
            _logicHandler.Received(1).SetSceneState((int)MenuState.InMenu);
        }

        [TestMethod]
        public void OnClick_btnBuyLivesMedium()
        {
            _getMoreLivesLogicProvider.OnClick("btnBuyLivesMedium");
            _gameEngineInterface.AppStoreService.Received(1).BuyProductByID(Constants.ProductNames.BuyLivesMedium);
            _logicHandler.Received(1).SetSceneState((int)MenuState.InMenu);
        }

        [TestMethod]
        public void OnClick_btnBuyLivesLarge()
        {
            _getMoreLivesLogicProvider.OnClick("btnBuyLivesLarge");
            _gameEngineInterface.AppStoreService.Received(1).BuyProductByID(Constants.ProductNames.BuyLivesLarge);
            _logicHandler.Received(1).SetSceneState((int)MenuState.InMenu);
        }
    }
}

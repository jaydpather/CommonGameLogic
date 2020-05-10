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
using ThirdEyeSoftware.GameLogic.StoreLogicService;

namespace GameLogicTest.LogicProviders.MenuLogicProviders
{
    [TestClass]
    public class GetMoreLivesLogicProviderTest
    {
        private GetMoreLivesLogicProvider _getMoreLivesLogicProvider;
        private ILogicHandler _logicHandler;
        private IGameEngineInterface _gameEngineInterface;
        private IDataLayer _dataLayer;
        private List<ProductInfoViewModel> _productInfoViewModels;

        private IGameObject _pnlGetMoreLives; 
        private IGameObject _btnCancel;
        private IGameObject _btnBuyLivesSmall;
        private IGameObject _btnBuyLivesMedium;
        private IGameObject _btnBuyLivesLarge;
        private IGameObject _txtCurrentLivesGameObject;
        private IText _txtCurrentLives;
        private Dictionary<string, Tuple<IText, IText>> _buttonAndSaveLabels = new Dictionary<string, Tuple<IText, IText>>();

        [TestInitialize]
        public void TestInitialize()
        {
            _logicHandler = Substitute.For<ILogicHandler>();
            _gameEngineInterface = Substitute.For<IGameEngineInterface>();
            _dataLayer = Substitute.For<IDataLayer>();

            var gameController = Substitute.For<IGameController>();
            _logicHandler.GameController.Returns(gameController);
            _productInfoViewModels = new List<ProductInfoViewModel>()
                {
                    new ProductInfoViewModel { ProductId = Constants.ProductNames.BuyLivesSmall, PriceString = "$0.99", SavePctString = string.Empty },
                    new ProductInfoViewModel { ProductId = Constants.ProductNames.BuyLivesMedium, PriceString = "$1.99", SavePctString = "SAVE 30%" },
                    new ProductInfoViewModel { ProductId = Constants.ProductNames.BuyLivesLarge, PriceString = "$2.99", SavePctString = "SAVE 40%" },
                };
            gameController.ProductsForUI.Returns(_productInfoViewModels);

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


            _buttonAndSaveLabels[Constants.ProductNames.BuyLivesSmall] = new Tuple<IText, IText>(Substitute.For<IText>(), Substitute.For<IText>());
            _btnBuyLivesSmall.GetComponent<IText>().Returns(_buttonAndSaveLabels[Constants.ProductNames.BuyLivesSmall].Item1);

            _buttonAndSaveLabels[Constants.ProductNames.BuyLivesMedium] = new Tuple<IText, IText>(Substitute.For<IText>(), Substitute.For<IText>());
            _btnBuyLivesMedium.GetComponent<IText>().Returns(_buttonAndSaveLabels[Constants.ProductNames.BuyLivesMedium].Item1);
            var savePctMediumGameObj = Substitute.For<IGameObject>();
            savePctMediumGameObj.GetComponent<IText>().Returns(_buttonAndSaveLabels[Constants.ProductNames.BuyLivesMedium].Item2);
            _gameEngineInterface.FindGameObject("txtBuyLivesMediumSavePct").Returns(savePctMediumGameObj);
            
                
            _buttonAndSaveLabels[Constants.ProductNames.BuyLivesLarge] = new Tuple<IText, IText>(Substitute.For<IText>(), Substitute.For<IText>());
            _btnBuyLivesLarge.GetComponent<IText>().Returns(_buttonAndSaveLabels[Constants.ProductNames.BuyLivesLarge].Item1);
            var savePctLargeGameObj = Substitute.For<IGameObject>();
            savePctLargeGameObj.GetComponent<IText>().Returns(_buttonAndSaveLabels[Constants.ProductNames.BuyLivesLarge].Item2);
            _gameEngineInterface.FindGameObject("txtBuyLivesLargeSavePct").Returns(savePctLargeGameObj);
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
            AssertPriceLabelsAreCorrect();
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

        private void AssertPriceLabelsAreCorrect()
        {
            var buttonTextFormatString = @"{0}
 LIVES 
{1}";

            var labels = _buttonAndSaveLabels[Constants.ProductNames.BuyLivesSmall];
            var expectedButtonText = string.Format(buttonTextFormatString, Constants.LivesPerProduct.Small, _productInfoViewModels[0].PriceString);
            Assert.AreEqual(expectedButtonText, labels.Item1.Text);
            Assert.AreEqual(_productInfoViewModels[0].SavePctString, labels.Item2.Text);

            labels = _buttonAndSaveLabels[Constants.ProductNames.BuyLivesMedium];
            expectedButtonText = string.Format(buttonTextFormatString, Constants.LivesPerProduct.Medium, _productInfoViewModels[1].PriceString);
            Assert.AreEqual(expectedButtonText, labels.Item1.Text);
            Assert.AreEqual(_productInfoViewModels[1].SavePctString, labels.Item2.Text);

            labels = _buttonAndSaveLabels[Constants.ProductNames.BuyLivesLarge];
            expectedButtonText = string.Format(buttonTextFormatString, Constants.LivesPerProduct.Large, _productInfoViewModels[2].PriceString);
            Assert.AreEqual(expectedButtonText, labels.Item1.Text);
            Assert.AreEqual(_productInfoViewModels[2].SavePctString, labels.Item2.Text);
        }
    }
}

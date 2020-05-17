using System.Collections.Generic;
using System.Linq;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders;
using ThirdEyeSoftware.GameLogic.StoreLogicService;

namespace GameLogic.LogicProviders.MenuLogicProviders
{
    public class GetMoreLivesLogicProvider : BaseLogicProvider
    {
        class ProductUIControls
        {
            public IText ButtonLabel { get; set; }
            public IText SavePctLabel { get; set; }

            public ProductUIControls(IText buttonLabel, IText savePctLabel)
            {
                ButtonLabel = buttonLabel;
                SavePctLabel = savePctLabel;
            }
        }

        private IGameObject _pnlGetMoreLives;
        private IGameObject _btnCancel;
        private IGameObject _btnBuyLivesSmall;
        private IGameObject _btnBuyLivesMedium;
        private IGameObject _btnBuyLivesLarge;
        private IText _txtCurrentLives;
        Dictionary<string, ProductUIControls> _productTextBoxMapping = new Dictionary<string, ProductUIControls>();

        public GetMoreLivesLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {
            
        }

        public override void OnClick(string sender)
        {
            switch(sender)
            {
                case "btnGetMoreLivesCancel":
                    btnCancel_Click();
                    break;
                case "btnBuyLivesSmall":
                    btnBuyLivesSmall_Click();
                    break;
                case "btnBuyLivesMedium":
                    btnBuyLivesMedium_Click();
                    break;
                case "btnBuyLivesLarge":
                    btnBuyLivesLarge_Click();
                    break;
            }
        }

        private void btnBuyLivesSmall_Click()
        {
            _gameEngineInterface.AppStoreService.BuyProductByID(Constants.ProductNames.BuyLivesSmall);
            _logicHandler.SetSceneState((int)MenuState.InMenu);
        }

        private void btnBuyLivesMedium_Click()
        {
            _gameEngineInterface.AppStoreService.BuyProductByID(Constants.ProductNames.BuyLivesMedium);
            _logicHandler.SetSceneState((int)MenuState.InMenu);
        }

        private void btnBuyLivesLarge_Click()
        {
            _gameEngineInterface.AppStoreService.BuyProductByID(Constants.ProductNames.BuyLivesLarge);
            _logicHandler.SetSceneState((int)MenuState.InMenu);
        }

        private void btnCancel_Click()
        {
            _logicHandler.SetSceneState((int)MenuState.InMenu);
        }

        public override void OnStart()
        {
            base.OnStart();

            _btnCancel = _gameEngineInterface.FindGameObject("btnGetMoreLivesCancel");
            _btnCancel.LogicHandler = _logicHandler;

            _btnBuyLivesSmall = _gameEngineInterface.FindGameObject("btnBuyLivesSmall");
            _btnBuyLivesSmall.LogicHandler = _logicHandler;

            _btnBuyLivesMedium = _gameEngineInterface.FindGameObject("btnBuyLivesMedium");
            _btnBuyLivesMedium.LogicHandler = _logicHandler;

            _btnBuyLivesLarge = _gameEngineInterface.FindGameObject("btnBuyLivesLarge");
            _btnBuyLivesLarge.LogicHandler = _logicHandler;

            var txtCurrentLivesGameObject = _gameEngineInterface.FindGameObject("txtCurrentLives");
            _txtCurrentLives = txtCurrentLivesGameObject.GetComponent<IText>();

            _populateProductTextBoxMapping();

            _pnlGetMoreLives = _gameEngineInterface.FindGameObject("pnlGetMoreLives");
            _pnlGetMoreLives.SetActive(false);
        }

        private void _populateProductTextBoxMapping()
        {
            var txtSavePctSmall = _gameEngineInterface.FindGameObject("txtBuyLivesSmallSavePct").GetComponent<IText>();
            var txtSavePctMedium = _gameEngineInterface.FindGameObject("txtBuyLivesMediumSavePct").GetComponent<IText>();
            var txtSavePctLarge = _gameEngineInterface.FindGameObject("txtBuyLivesLargeSavePct").GetComponent<IText>();

            _productTextBoxMapping[Constants.ProductNames.BuyLivesSmall] = new ProductUIControls(_btnBuyLivesSmall.GetComponent<IText>(), txtSavePctSmall);
            _productTextBoxMapping[Constants.ProductNames.BuyLivesMedium] = new ProductUIControls(_btnBuyLivesMedium.GetComponent<IText>(), txtSavePctMedium);
            _productTextBoxMapping[Constants.ProductNames.BuyLivesLarge] = new ProductUIControls(_btnBuyLivesLarge.GetComponent<IText>(), txtSavePctLarge);
        }

        public override void OnActivate()
        {
            _pnlGetMoreLives.SetActive(true);

            _txtCurrentLives.Text = "REMAINING LIVES: " + _dataLayer.GetNumLivesRemaining();

            SetPriceLabels(_logicHandler.GameController.ProductsForUI);
        }

        //todo: this List should have really been a dictionary, keyed by product ID
        private void SetPriceLabels(List<ProductInfoViewModel> products)
        {
            if(products == null || products.Count == 0)
            {
                LogToDebugOutput("Could not load products from Google Play store. Please try again later.");
            }
            else
            {
                var buttonTextFormatString = @"{0}
 LIVES 
{1}";
                var smallProduct = products.Single(x => x.ProductId == Constants.ProductNames.BuyLivesSmall);
                var medProduct = products.Single(x => x.ProductId == Constants.ProductNames.BuyLivesMedium);
                var largeProduct = products.Single(x => x.ProductId == Constants.ProductNames.BuyLivesLarge);

                _productTextBoxMapping[Constants.ProductNames.BuyLivesSmall].ButtonLabel.Text = string.Format(buttonTextFormatString, Constants.LivesPerProduct.Small, smallProduct.PriceString);
                _productTextBoxMapping[Constants.ProductNames.BuyLivesMedium].ButtonLabel.Text = string.Format(buttonTextFormatString, Constants.LivesPerProduct.Medium, medProduct.PriceString);
                _productTextBoxMapping[Constants.ProductNames.BuyLivesLarge].ButtonLabel.Text = string.Format(buttonTextFormatString, Constants.LivesPerProduct.Large, largeProduct.PriceString);

                _productTextBoxMapping[Constants.ProductNames.BuyLivesSmall].SavePctLabel.Text = smallProduct.SavePctString;
                _productTextBoxMapping[Constants.ProductNames.BuyLivesMedium].SavePctLabel.Text = medProduct.SavePctString;
                _productTextBoxMapping[Constants.ProductNames.BuyLivesLarge].SavePctLabel.Text = largeProduct.SavePctString;

            }
        }

        public override void OnDeActivate()
        {
            _pnlGetMoreLives.SetActive(false);
        }

        //todo: this needs to be removed, right? no longer planning to use this
        public void OnPricesLoaded(List<ProductInfoViewModel> products)
        {

        }
    }
}

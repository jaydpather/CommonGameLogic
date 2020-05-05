using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.StoreLogicService;

namespace GameLogicTest.StoreLogicServiceTest
{
    [TestClass]
    public class StoreLogicServiceTest : TestBase
    {
        private StoreLogicService _storeLogicService;
        private IDataLayer _dataLayer;

        [TestInitialize]
        public void TestInitialize()
        {
            _storeLogicService = StoreLogicService.Instance; //todo ut: how can we re-initialize _storeLogicService if it's a singleton?

            _dataLayer = Substitute.For<IDataLayer>();
            _storeLogicService.DataLayer = _dataLayer;
        }

        [TestMethod]
        public void GenerateSavePctString_Success()
        {
            CallPrivateMethod(_storeLogicService, "GenerateSavePctString", new object[] { 45M });

        }

        [TestMethod]
        public void OnAppStorePurchaseSucceeded_Small()
        {
            _storeLogicService.OnAppStorePurchaseSucceeded(Constants.ProductNames.BuyLivesSmall);
            _dataLayer.Received().IncrementNumLivesRemaining(Constants.LivesPerProduct.Small);
        }

        [TestMethod]
        public void OnAppStorePurchaseSucceeded_Medium()
        {
            _storeLogicService.OnAppStorePurchaseSucceeded(Constants.ProductNames.BuyLivesMedium);
            _dataLayer.Received().IncrementNumLivesRemaining(Constants.LivesPerProduct.Medium);
        }

        [TestMethod]
        public void OnAppStorePurchaseSucceeded_Large()
        {
            //todo: use nUnit parameters to test the small/medium/large cases in one method
            _storeLogicService.OnAppStorePurchaseSucceeded(Constants.ProductNames.BuyLivesLarge);
            _dataLayer.Received().IncrementNumLivesRemaining(Constants.LivesPerProduct.Large);
        }

        [TestMethod]
        public void OnAppStorePurchaseSucceeded_Other()
        {
            _storeLogicService.OnAppStorePurchaseSucceeded("OtherProductName");

            _dataLayer.DidNotReceive().IncrementNumLivesRemaining(Arg.Any<int>());
        }
    }
}

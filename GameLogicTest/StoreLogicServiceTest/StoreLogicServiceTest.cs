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
            var inputs = new[]          { 45M,   60.1M, 35.9M };
            var expectedOutputs = new[] { "45%", "60%", "35%" };

            if (inputs.Length != expectedOutputs.Length)
                Assert.Fail("UT is set up incorrectly - number of inputs does not match number of expected outputs");

            for(var i=0; i<inputs.Length; i++)
            {
                var curInput = inputs[i];
                var curExpectedOutput = expectedOutputs[i];

                CallPrivateMethod(_storeLogicService, "GenerateSavePctString", new object[] { curInput });
                Assert.AreEqual(curExpectedOutput, curInput);
            }
        }

        [TestMethod]
        public void SetProductQuantity()
        {
            var smallProduct = new ProductInfo { ProductId = Constants.ProductNames.BuyLivesSmall };
            var mediumProduct = new ProductInfo { ProductId = Constants.ProductNames.BuyLivesMedium };
            var largeProduct = new ProductInfo { ProductId = Constants.ProductNames.BuyLivesLarge };
            var invalidProduct = new ProductInfo { ProductId = "invalid", Quantity = -1 };

            var products = new List<ProductInfo>
            {
                smallProduct,
                mediumProduct,
                largeProduct,
                invalidProduct
            };

            CallPrivateMethod(_storeLogicService, "SetProductQuantity", new object[] { products });

            Assert.AreEqual(Constants.LivesPerProduct.Small, smallProduct.Quantity);
            Assert.AreEqual(Constants.LivesPerProduct.Medium, mediumProduct.Quantity);
            Assert.AreEqual(Constants.LivesPerProduct.Large, largeProduct.Quantity);
            Assert.AreEqual(-1, invalidProduct.Quantity); //verify this wasn't changed
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

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
        private const string VALIDATE_PRODUCTS_MISSING = "missing";
        private const string VALIDATE_PRODUCTS_EXTRA = "extra";

        [TestInitialize]
        public void TestInitialize()
        {
            _storeLogicService = StoreLogicService.Instance; //todo ut: how can we re-initialize _storeLogicService if it's a singleton?

            _dataLayer = Substitute.For<IDataLayer>();
            _storeLogicService.DataLayer = _dataLayer;
        }

        [TestMethod]
        public void ValidateProducts_Success()
        {
            var products = new List<ProductInfo>()
            {
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesLarge },
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesMedium},
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesSmall }
            };

            var results = _storeLogicService.ValidateProducts(products);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void ValidateProducts_Missing()
        {
            var products = new List<ProductInfo>()
            {
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesLarge },
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesMedium},
            };

            var results = _storeLogicService.ValidateProducts(products);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Contains(VALIDATE_PRODUCTS_MISSING));
        }

        [TestMethod]
        public void ValidateProducts_Extra()
        {
            var products = new List<ProductInfo>()
            {
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesLarge },
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesMedium},
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesSmall },
                new ProductInfo { ProductId = "fake product" },
            };

            var results = _storeLogicService.ValidateProducts(products);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Contains(VALIDATE_PRODUCTS_MISSING));
        }

        [TestMethod]
        public void ValidateProducts_MissingAndExtra()
        {
            var products = new List<ProductInfo>()
            {
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesLarge },
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesMedium},
                new ProductInfo { ProductId = "fake product" },
            };

            var results = _storeLogicService.ValidateProducts(products);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results.Count(x => x.Contains(VALIDATE_PRODUCTS_MISSING)));
            Assert.AreEqual(1, results.Count(x => x.Contains(VALIDATE_PRODUCTS_EXTRA)));
        }

        [TestMethod]
        public void FindSmallestProductInfo_Success()
        {
            var smallProductInfo = new ProductInfo { ProductId = Constants.ProductNames.BuyLivesSmall };

            var productInfos = new List<ProductInfo>()
            {
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesLarge },
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesMedium },
                smallProductInfo,
            };

            var result = CallPrivateMethod<ProductInfo>(_storeLogicService, "FindSmallestProductInfo", new object[] { productInfos });

            Assert.AreEqual(smallProductInfo, result);
        }

        [TestMethod]
        public void FindSmallestProductInfo_NotFound()
        {
            var productInfos = new List<ProductInfo>()
            {
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesLarge },
                new ProductInfo { ProductId = Constants.ProductNames.BuyLivesMedium },
            };

            Action testFn = () => CallPrivateMethod<ProductInfo>(_storeLogicService, "FindSmallestProductInfo", new object[] { productInfos });

            Assert.ThrowsException<InvalidOperationException>(testFn);
        }

        [TestMethod]
        public void CalculateSavePercent_FiftyPercent()
        {
            var smallProduct = new ProductInfo { Price = 1M, Quantity = 1 };
            var bulkProduct = new ProductInfo { Price = 5M, Quantity = 10 };

            var result = CallPrivateMethod<decimal>(_storeLogicService, "CalculateSavePercent", new object[] { smallProduct, bulkProduct });

            Assert.AreEqual(50M, result);
        }

        [TestMethod]
        public void CalculateSavePercent_OneThird()
        {
            //you'd have to pay $3 to get 3 lives with the small product
            //but you can pay $2 to get 3 lives with the bulk product
            //therefore, the user saves 33.333...%

            var smallProduct = new ProductInfo { Price = 1M, Quantity = 1 };
            var bulkProduct = new ProductInfo { Price = 2M, Quantity = 3 };

            var result = CallPrivateMethod<decimal>(_storeLogicService, "CalculateSavePercent", new object[] { smallProduct, bulkProduct });

            var expectedResult = (1M / 3M) * 100;
            Assert.AreEqual(expectedResult, result);
            // was Assert.AreEqual(50M, result); now gives a correct result
        }

        [TestMethod]
        public void CalculateSavePercent_Zero()
        {
            //you'd have to pay $3 to get 3 lives with the small product
            //but you can pay $2 to get 3 lives with the bulk product
            //therefore, the user saves 33.333...%

            var smallProduct = new ProductInfo { Price = 1M, Quantity = 1 };

            var result = CallPrivateMethod<decimal>(_storeLogicService, "CalculateSavePercent", new object[] { smallProduct, smallProduct });

            Assert.AreEqual(0M, result);
        }

        [TestMethod]
        public void CalculateSavePercent_Negative()
        {
            //in this case, the bulk product is actually twice as expensive per quantity
            //so, the user saves -100%
            //hopefully this will never happen in prod. this test is just to check that our formula is correct

            var smallProduct = new ProductInfo { Price = 1M, Quantity = 1 };
            var bulkProduct = new ProductInfo { Price = 2M, Quantity = 1 };

            var result = CallPrivateMethod<decimal>(_storeLogicService, "CalculateSavePercent", new object[] { smallProduct, bulkProduct });

            Assert.AreEqual(-100M, result);
        }

        [TestMethod]
        public void CalculateSavePercent_DifferentQuantity()
        {
            var smallProduct = new ProductInfo { Price = 1M, Quantity = 3 };
            var bulkProduct = new ProductInfo { Price = 5M, Quantity = 50 };

            var result = CallPrivateMethod<decimal>(_storeLogicService, "CalculateSavePercent", new object[] { smallProduct, bulkProduct });

            Assert.AreEqual(70M, result);
        }
        [TestMethod]
        public void GenerateSavePctString()
        {
            var inputs = new[]          { 45M,   60.1M, 35.9M };
            var expectedOutputs = new[] { "SAVE 45%", "SAVE 60%", "SAVE 35%" };

            if (inputs.Length != expectedOutputs.Length)
                Assert.Fail("UT is set up incorrectly - number of inputs does not match number of expected outputs");

            for(var i=0; i<inputs.Length; i++)
            {
                var curInput = inputs[i];
                var curExpectedOutput = expectedOutputs[i];

                var actualOutput = CallPrivateMethod<string>(_storeLogicService, "GenerateSavePctString", new object[] { curInput });
                Assert.AreEqual(curExpectedOutput, actualOutput);
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

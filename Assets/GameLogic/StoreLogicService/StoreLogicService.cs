using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThirdEyeSoftware.GameLogic.StoreLogicService
{
    public interface IStoreLogicService
    {
        IDataLayer DataLayer { get; set; }
        void OnAppStorePurchaseSucceeded(string productId);
        Action<string> LogToDebugOutput { get; set; }
        List<string> ValidateProducts(List<ProductInfo> products);
        void OnProductsLoaded(List<ProductInfo> validatedProducts);
        Action<List<ProductInfoViewModel>> OnProductsConverted { get; set; }
    }

    public class StoreLogicService : IStoreLogicService
    {
        private static readonly StoreLogicService _instance = new StoreLogicService();

        private ProductInfo FindSmallestProductInfo (List<ProductInfo>productInfos)
        {
              return null;
        }

        private decimal CalculateSavePercent(ProductInfo smallPackage, ProductInfo bulkPackage)
        {
            decimal smallBulkPrice = smallPackage.Price / smallPackage.Quantity * bulkPackage.Quantity;
            decimal bulkPrice = bulkPackage.Price;
            decimal packagePrice = bulkPrice / smallBulkPrice;
            decimal priceSaved = (1 - packagePrice) * 100;
            decimal retVal = priceSaved;
            return retVal;
           
        }

        private string GenerateSavePctString(decimal savePct)
        {
            string retVal;
            if (savePct <= 0) 
            {
                retVal = string.Empty;
            } 
            else 
            {
                retVal =  $"SAVE { Math.Truncate(savePct)}%";
            }

            return retVal;
                       
        }

        private void SetProductQuantity(List<ProductInfo> products)
             /* use Constants.ProductNames to determine which ProductInfo is small/medium/large. (compare to ProductInfo.ProductId)
                use Constants.LivesPerProduct to know how many lives are in small/medium/large.
                use a Dictionary<string, int> to create mapping from Product Id -> Number of Lives
                e.g.myDictionary["small"] = 10;
                now you don't need if statements: if(productInfo.ProductId == Constants.ProductNames.Small */

        {
            Dictionary<string, int> dicNumberofLives = new Dictionary<string, int>();
            dicNumberofLives.Add(Constants.ProductNames.BuyLivesSmall, Constants.LivesPerProduct.Small);
            dicNumberofLives.Add(Constants.ProductNames.BuyLivesMedium, Constants.LivesPerProduct.Medium);
            dicNumberofLives.Add(Constants.ProductNames.BuyLivesLarge, Constants.LivesPerProduct.Large);
            
            foreach (var curProductInfo in products)
            {
                if (curProductInfo.ProductId == Constants.ProductNames.BuyLivesSmall)
                {
                    curProductInfo.Quantity = Constants.LivesPerProduct.Small;
                }
                else if (curProductInfo.ProductId == Constants.ProductNames.BuyLivesMedium)
                {
                    curProductInfo.Quantity = Constants.LivesPerProduct.Medium;
                }
                else if (curProductInfo.ProductId == Constants.ProductNames.BuyLivesLarge)
                {
                    curProductInfo.Quantity = Constants.LivesPerProduct.Large;
                }
                //curProductInfo.Quantity = dicNumberofLives[element.ProductId]; to use dictionary  
            }

        }

        public static StoreLogicService Instance
        {
            get
            {
                return _instance;
            }
        }

        public IDataLayer DataLayer
        {
            get;
            set;
        }

        public Action<string> LogToDebugOutput
        {
            get;
            set;
        }

        public Action<List<ProductInfoViewModel>> OnProductsConverted
        {
            get;
            set;
        }

        public void OnAppStorePurchaseSucceeded(string productId)
        {
            //todo real game: extract method: SaveAdState: 
            try
            {
                //LogToDebugOutput("StoreLogicService.OnAppStorePurchaseSucceeded()");
                
                switch (productId)
                {
                    case Constants.ProductNames.BuyLivesSmall:
                        DataLayer.IncrementNumLivesRemaining(Constants.LivesPerProduct.Small);
                        break;
                    case Constants.ProductNames.BuyLivesMedium:
                        DataLayer.IncrementNumLivesRemaining(Constants.LivesPerProduct.Medium);
                        break;
                    case Constants.ProductNames.BuyLivesLarge:
                        DataLayer.IncrementNumLivesRemaining(Constants.LivesPerProduct.Large);
                        break;
                }
            }
            catch
            {
                //todo real game: log exception. (log it in some way that the user can attach the log file if they have a support issue)

                throw;
            }
        }

        public List<string> ValidateProducts(List<ProductInfo> products)
        {
            return null;
        }

        public void OnProductsLoaded(List<ProductInfo> validatedProducts)
        {
            
        }
    }
    
}

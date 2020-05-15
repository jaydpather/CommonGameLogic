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
            decimal standardPrice = smallPackage.Price;
            decimal bulkPrice = standardPrice * bulkPackage.Price;
            decimal packagePrice = bulkPrice / bulkPackage.Quantity;
            decimal priceSaved = (1 - packagePrice) * 100;
            return priceSaved;
           
        }

        private string GenerateSavePctString(decimal savePct)
        {
            string saveResult;
            if (savePct <= 0) 
            {
              saveResult = string.Empty;
            } 
            else 
            {
              saveResult =  $"SAVE { Math.Truncate(savePct)}%";
            }

            return saveResult;
                       
        }

        private void SetProductQuantity(List<ProductInfo> products)
        {
            throw new NotImplementedException();
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
        
        
    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
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
            List<ProductInfoViewModel> listProductInfoViewModel = new List<ProductInfoViewModel>();

            SetProductQuantity(validatedProducts);

            foreach (var curProductInfo in validatedProducts)
            {
                ProductInfoViewModel viewModel = new ProductInfoViewModel();
                listProductInfoViewModel.Add(viewModel);
                viewModel.PriceString = curProductInfo.PriceString;
                viewModel.ProductId = curProductInfo.ProductId;
                decimal savepct = CalculateSavePercent(curProductInfo, curProductInfo);
                viewModel.SavePctString = GenerateSavePctString(savepct);

                return;
            }

            if (validatedProducts != null)
            {
                OnProductsConverted.Invoke(listProductInfoViewModel);
            }
           
        }
    }
    
}

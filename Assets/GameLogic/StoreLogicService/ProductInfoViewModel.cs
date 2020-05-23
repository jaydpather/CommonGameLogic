using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdEyeSoftware.GameLogic.StoreLogicService
{
    public class ProductInfoViewModel
    {
        public string PriceString { get; set; }
        public string SavePctString { get; set; } //e.g. "45%"
        public string ProductId { get; set; }
    }
}

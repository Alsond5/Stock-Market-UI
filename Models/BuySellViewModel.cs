using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMarketUI.Models
{
    public class BuySellViewModel
    {
        public int StockId { get; set; }
        public string? StockSymbol { get; set; }
        public int Quantity { get; set; }
    }
}
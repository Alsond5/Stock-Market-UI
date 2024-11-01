using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMarketUI.Models
{
    public class Stock
    {
        public int StockId { get; set; }
        public required string StockSymbol { get; set; }
        public required string StockName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
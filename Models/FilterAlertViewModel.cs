using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMarketUI.Models
{
    public class FilterAlertViewModel
    {
        public int StockId { get; set; }
        public string StockSymbol { get; set; } = string.Empty;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMarketUI.Models
{
    public class Holding
    {
        public int HoldingId { get; set; }
        public int Quantity { get; set; }
        public int PortfolioId { get; set; }
        public required Stock Stock { get; set; }
    }
}
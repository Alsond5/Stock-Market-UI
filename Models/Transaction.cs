using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMarketUI.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Commission { get; set; }
        public DateTime TransactionDate { get; set; }

        public int UserId { get; set; }
        public required Stock Stock { get; set; }
    }
}
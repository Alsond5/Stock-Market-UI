using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockMarketUI.Models
{
    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public required string Token { get; set; }

        [JsonPropertyName("tokenType")]
        public required string TokenType { get; set; }

        [JsonPropertyName("expiration")]
        public DateTime Expiration { get; set; }
    }
}
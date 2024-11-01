using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarketUI.Models;

namespace StockMarketUI.Controllers
{
    public class TradeController(ILogger<TradeController> logger, IHttpClientFactory httpClient) : Controller
    {
        private readonly ILogger<TradeController> _logger = logger;
        private readonly IHttpClientFactory _httpClient = httpClient;

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");
            var tokenType = HttpContext.Session.GetString("TokenType");
            
            if (token is null || tokenType is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);

            var response = await client.GetAsync("/api/stocks/all");

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var stocks = await response.Content.ReadFromJsonAsync<List<Stock>>();

            return View(stocks);
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Buy()
        {
            return View();
        }

        public IActionResult Sell()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        public IActionResult Portfolio()
        {
            return View();
        }
    }
}
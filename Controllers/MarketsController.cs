using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarketUI.Models;

namespace StockMarketUI.Controllers
{
    public class MarketsController(ILogger<MarketsController> logger, IHttpClientFactory httpClient) : Controller
    {
        private readonly ILogger<MarketsController> _logger = logger;
        private readonly IHttpClientFactory _httpClient = httpClient;
        
        public IActionResult Index()
        {
            // redirect to overview

            return RedirectToAction("Overview");
        }

        public async Task<IActionResult> Overview()
        {
            var client = _httpClient.CreateClient("StockMarketAPI");
            var response = await client.GetAsync("/api/stocks/all");

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var markets = await response.Content.ReadFromJsonAsync<List<Stock>>();

            return View(markets);
        }

        public async Task<IActionResult> Price(string id)
        {
            var client = _httpClient.CreateClient("StockMarketAPI");
            var response = await client.GetAsync($"/api/stocks/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var stock = await response.Content.ReadFromJsonAsync<Stock>();

            return View(stock);
        }
    }
}
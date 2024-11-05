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
                ViewBag.Stock = null;
                return View();
            }

            var stock = await response.Content.ReadFromJsonAsync<Stock>();
            ViewBag.Stock = stock;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Buy(BuySellViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid buy attempt");
                ViewBag.Success = "error";

                return RedirectToAction("Price", new { id = "AEFES" });
            }

            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var response = await client.PutAsJsonAsync($"/api/holdings/buy", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Invalid buy attempt");
                ViewBag.Success = "error";

                response = await client.GetAsync($"/api/stocks/{model.StockSymbol}");

                var stock = await response.Content.ReadFromJsonAsync<Stock>();
                ViewBag.Stock = stock;

                return View("Price", model);
            }

            var holding = await response.Content.ReadFromJsonAsync<Holding>();

            ViewBag.Success = "success";
            ViewBag.Stock = holding!.Stock;

            return View("Price", model);
        }

        [HttpPost]
        public async Task<IActionResult> Sell(BuySellViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid buy attempt");
                ViewBag.Success = "error";

                return RedirectToAction("Price", new { id = model.StockSymbol});
            }

            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var response = await client.PutAsJsonAsync($"/api/holdings/sell", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Invalid buy attempt");
                ViewBag.Success = "error";

                response = await client.GetAsync($"/api/stocks/{model.StockSymbol}");

                var stock = await response.Content.ReadFromJsonAsync<Stock>();
                ViewBag.Stock = stock;

                return View("Price", model);
            }

            var holding = await response.Content.ReadFromJsonAsync<Holding>();

            ViewBag.Success = "success";
            ViewBag.Stock = holding!.Stock;

            return View("Price", model);
        }
    }
}
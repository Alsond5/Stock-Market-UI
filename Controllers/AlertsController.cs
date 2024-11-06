using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarketUI.Models;

namespace StockMarketUI.Controllers
{
    public class AlertsController(IHttpClientFactory httpClient) : Controller
    {
        private readonly IHttpClientFactory _httpClient = httpClient;

        public IActionResult Index()
        {
            return RedirectToAction("overview", "alerts");
        }
        
        public async Task<IActionResult> Overview()
        {
            var stockId = Request.Query.ContainsKey("stock-id") ? Request.Query["stock-id"].ToString() : null;

            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var url = stockId != null ? $"/api/alerts/@me/alerts/stock/{int.Parse(stockId)}" : "/api/alerts/@me/alerts";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var alerts = await response.Content.ReadFromJsonAsync<List<Alert>>();
            ViewBag.Alerts = alerts;

            response = await client.GetAsync("/api/stocks/all");

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var stocks = await response.Content.ReadFromJsonAsync<List<Stock>>();
            ViewBag.Stocks = stocks;

            return View();
        }

        public async Task<IActionResult> Stock(int id)
        {
            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var response = await client.GetAsync($"/api/alerts/@me/alerts/stock/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Stock = null;
                return View();
            }

            var alerts = await response.Content.ReadFromJsonAsync<List<Alert>>();
            ViewBag.Alerts = alerts;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AlertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid alert attempt");
                ModelState.AddModelError(string.Empty, "Invalid alert attempt");
                return RedirectToAction("Overview");
            }

            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var response = await client.PostAsJsonAsync("/api/alerts/@me/alerts", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to create alert");
                return RedirectToAction("Overview");
            }

            return RedirectToAction("Overview");
        }

        [HttpPost]
        public async Task<IActionResult> Update(AlertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid alert attempt");
                return View();
            }

            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var response = await client.PutAsJsonAsync($"/api/alerts/@me/alerts", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update alert");
                return View();
            }

            return RedirectToAction("Overview");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var response = await client.DeleteAsync($"/api/alerts/@me/alerts/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete alert");
            }

            return RedirectToAction("Overview");
        }
    }
}
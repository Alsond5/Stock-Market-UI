using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMarketUI.Models;

namespace StockMarketUI.Controllers
{
    [Authorize]
    public class ProfileController(IHttpClientFactory httpClient) : Controller
    {
        private readonly IHttpClientFactory _httpClient = httpClient;

        public async Task<IActionResult> Index()
        {
            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var token = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);

            var response = await client.GetAsync("/api/holdings/@me/all");
            if (!response.IsSuccessStatusCode) return View();

            var holdings = await response.Content.ReadFromJsonAsync<List<Holding>>();
            if (holdings == null) return View();

            var userInfo = ViewBag.userInfo as UserInfo;

            var estimatedBalance = holdings.Sum(h => h.Stock.Price * h.Quantity);
            ViewBag.EstimatedBalance = estimatedBalance + userInfo!.Balance;
            ViewBag.Holdings = holdings;

            return View();
        }

        public async Task<IActionResult> Transactions(string? id)
        {
            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var token = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);

            var url = "/api/transactions/@me/";
            var action = (id != null) ? $"stock/{id}" : "all";
            url += action;

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return View(new List<Transaction>());

            var transactions = await response.Content.ReadFromJsonAsync<List<Transaction>>();
            if (transactions == null) return View(new List<Transaction>());
            
            transactions.Reverse();

            return View(transactions);
        }

        public async Task<IActionResult> ExportTransactions()
        {
            var startDate = Request.Query.ContainsKey("startDate") ? Request.Query["startDate"].ToString() : "2021-01-01";
            var endDate = Request.Query.ContainsKey("endDate") ? Request.Query["endDate"].ToString() : "2024-12-31";
            
            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var token = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);

            var response = await client.GetAsync($"/api/transactions/download?startDate={startDate}&endDate={endDate}");
            if (!response.IsSuccessStatusCode) return StatusCode(500);
            
            var transactions = await response.Content.ReadAsStreamAsync();
            if (transactions == null) return StatusCode(500);

            return File(transactions, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarketUI.Models;

namespace StockMarketUI.Controllers
{
    public class AdminController(IHttpClientFactory clientFactory) : Controller
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Models = new()
        {
            { "alerts", new Dictionary<string, string> {
                { "STOCK SYMBOL", "stockSymbol" },
                { "LOWER LIMIT", "lowerLimit" },
                { "UPPER LIMIT", "upperLimit" },
            } },
            { "balances", new Dictionary<string, string> {
                { "USERNAME", "username" },
                { "EMAIL", "email" },
                { "BALANCE", "amount" },
            } },
            { "stocks", new Dictionary<string, string> {
                { "STOCK NAME", "stockName" },
                { "STOCK SYMBOL", "stockSymbol" },
                { "PRICE", "price" },
                { "STATUS", "isActive" },
                { "LAST UPDATE", "lastUpdated" }
            } },
            { "transactions", new Dictionary<string, string> {
                { "STOCK SYMBOL", "stock.stockSymbol" },
                { "USER ID", "userId" },
                { "TRANSACTION TYPE", "transactionType" },
                { "QUANTITY", "quantity" },
                { "PRICE", "pricePerUnit" },
                { "TRANSACTION DATE", "transactionDate" }
            } },
            { "holdings", new Dictionary<string, string> {
                { "STOCK SYMBOL", "stock.stockSymbol" },
                { "QUANTITY", "quantity" },
                { "PORTFOLIO ID", "portfolioId" },
            } },
            { "coupons", new Dictionary<string, string> {
                { "COUPON CODE", "code" },
                { "AMOUNT", "amount" },
                { "IS REDEEMED", "isReedemed" },
                { "EXPIRY DATE", "expiryDate" }
            } },
            { "configs", new Dictionary<string, string> {
                { "KEY", "key" },
                { "VALUE", "value" }
            } }
        };
        private readonly IHttpClientFactory _clientFactory = clientFactory;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users(int? id)
        {
            if (id != null) return RedirectToAction("EditUser", new { id = id });

            var tokenType = HttpContext.Session.GetString("tokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _clientFactory.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var response = await client.GetAsync($"/api/admin/users");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("index", "admin");
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserInfo>>();
            ViewBag.Users = users;

            return View();
        }

        public async Task<IActionResult> EditUser(int id)
        {
            return View();
        }

        public async Task<IActionResult> StockMarket(string id)
        {
            if (!Models.TryGetValue(id, out Dictionary<string, string>? value))
            {
                return RedirectToAction("index", "admin");
            }

            var tokenType = HttpContext.Session.GetString("tokenType") ?? "Bearer";
            var accessToken = HttpContext.Session.GetString("Token");

            var client = _clientFactory.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var response = await client.GetAsync($"/api/{id}/all");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("index", "admin");
            }

            var data = await response.Content.ReadFromJsonAsync<List<JsonObject>>();
            if (data == null) return RedirectToAction("index", "admin");

            data.Reverse();

            ViewBag.Data = data;
            ViewBag.ModelTitle = id;
            ViewBag.ModelAttributes = value;
            
            return View();
        }

        public IActionResult Stocks()
        {
            return View();
        }

        public IActionResult EditStock(string id)
        {
            return View();
        }

        public IActionResult Transactions()
        {
            return View();
        }
    }
}
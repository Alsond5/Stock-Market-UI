using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarketUI.Models;

namespace StockMarketUI.Controllers
{
    public class CouponsController(IHttpClientFactory httpClient) : Controller
    {
        private readonly IHttpClientFactory _httpClient = httpClient;

        [HttpPost]
        public async Task<IActionResult> Redeem(Coupon coupon)
        {
            var tokenType = HttpContext.Session.GetString("TokenType") ?? "Bearer";
            var token = HttpContext.Session.GetString("Token");

            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);

            var response = await client.PutAsJsonAsync("/api/coupons/redeem", coupon);
            if (!response.IsSuccessStatusCode) return RedirectToAction("Index", "Profile");

            HttpContext.Session.Remove("UserInfo");

            return RedirectToAction("Index", "Profile");
        }
    }
}
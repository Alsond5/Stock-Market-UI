using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockMarketUI.Models;

namespace StockMarketUI.Filters
{
    public class UserInfoActionFilter(IHttpClientFactory httpClient) : ActionFilterAttribute
    {
        private readonly IHttpClientFactory _httpClient = httpClient;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine("UserInfoActionFilter executing...");

            var tokenType = context.HttpContext.Session.GetString("TokenType");
            var token = context.HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(tokenType) || string.IsNullOrEmpty(token))
            {
                await next();
                return;
            }

            if (!GetUserInfoFromSession(context)) await GetUserInfo(context, tokenType, token);

            await next();
        }

        private static bool GetUserInfoFromSession(ActionExecutingContext context)
        {
            var userInfoJson = context.HttpContext.Session.GetString("UserInfo");

            if (string.IsNullOrEmpty(userInfoJson))
            {
                return false;
            }

            var userInfo = JsonSerializer.Deserialize<UserInfo>(userInfoJson);

            if (userInfo is null)
            {
                return false;
            }

            if (context.Controller is Controller controller)
            {
                controller.ViewBag.UserInfo = userInfo;
            }

            return true;
        }

        private async Task<bool> GetUserInfo(ActionExecutingContext context, string tokenType, string token)
        {
            var client = _httpClient.CreateClient("StockMarketAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);
            
            var response = await client.GetAsync("/api/users/@me");

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();

            if (userInfo is null)
            {
                return false;
            }

            if (context.Controller is Controller controller)
            {
                controller.ViewBag.UserInfo = userInfo;
            }
            context.HttpContext.Session.SetString("UserInfo", JsonSerializer.Serialize(userInfo));
            
            return true;
        }
    }
}
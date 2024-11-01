using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarketUI.Models;

namespace StockMarketUI.Controllers;

public class AuthController(ILogger<AuthController> logger, IHttpClientFactory httpClient) : Controller
{
    private readonly ILogger<AuthController> _logger = logger;
    private readonly IHttpClientFactory _httpClient = httpClient;

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        _logger.LogInformation($"Login attempt for {model.UsernameOrEmail} with password {model.Password}");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClient.CreateClient("StockMarketAPI");
        var response = await client.PostAsJsonAsync("/api/auth/login", model);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

        if (token is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }

        HttpContext.Session.SetString("Token", token.Token);
        HttpContext.Session.SetString("TokenType", token.TokenType);
        HttpContext.Session.SetString("Expiration", token.Expiration.ToString("o"));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.Token);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        _logger.LogInformation($"Register attempt for {model.Username} with email {model.Email}");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClient.CreateClient("StockMarketAPI");
        var response = await client.PostAsJsonAsync("/api/auth/register", model);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Invalid register attempt");
            return View(model);
        }

        return RedirectToAction("Login");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
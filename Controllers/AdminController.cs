using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StockMarketUI.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
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
using CurrencyConverter.Models;
using CurrencyConverter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CurrencyConverter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExchangeRateService _exchangeRateService;
        public HomeController(ILogger<HomeController> logger, ExchangeRateService exchangeRateService)
        {
            _logger = logger;
            _exchangeRateService = exchangeRateService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

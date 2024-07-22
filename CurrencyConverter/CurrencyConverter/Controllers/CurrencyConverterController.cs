using CurrencyConverter.Data;
using CurrencyConverter.Models;
using CurrencyConverter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CurrencyConverter.Controllers
{
    public class CurrencyConverterController : Controller
    {
        private readonly ExchangeRateService _exchangeRateService;
        private readonly ApplicationDbContext _context;

        public CurrencyConverterController(ExchangeRateService exchangeRateService, ApplicationDbContext context)
        {
            _exchangeRateService = exchangeRateService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new CurrencyConvert();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Convert(CurrencyConvert model, bool saveFavorite)
        {
            var rate = await _exchangeRateService.GetExchangeRateAsync(model.BaseCurrency, model.TargetCurrency);

            if (rate > 0)
            {
                model.ConvertedAmount = model.Amount * rate;

                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var history = new CurrencyConvert
                    {
                        UserId = userId,
                        BaseCurrency = model.BaseCurrency,
                        TargetCurrency = model.TargetCurrency,
                        Amount = model.Amount,
                        ConvertedAmount = model.ConvertedAmount,
                        ConversionDate = DateTime.UtcNow
                    };
                    _context.CurrencyConverts.Add(history);
                    await _context.SaveChangesAsync();

                    if (saveFavorite)
                    {
                        var favorite = new FavoriteCurrencyPair
                        {
                            UserId = userId,
                            BaseCurrency = model.BaseCurrency,
                            TargetCurrency = model.TargetCurrency
                        };
                        _context.FavoriteCurrencyPairs.Add(favorite);
                        await _context.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, convertedAmount = model.ConvertedAmount });
            }
            else
            {
                return Json(new { success = false, errorMessage = "Invalid target currency." });
            }
        }

        public async Task<IActionResult> Favorites()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favorites = await _context.FavoriteCurrencyPairs
                    .Where(f => f.UserId == userId)
                    .ToListAsync();
                return View(favorites);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            var rate = await _exchangeRateService.GetExchangeRateAsync(baseCurrency, targetCurrency);

            if (rate > 0)
            {
                return Json(new { rate });
            }

            return Json(new { error = "Currency not found." });
        }
        [HttpGet]
        public IActionResult GetCountries()
        {
            var countriesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "countries.json");

            if (!System.IO.File.Exists(countriesPath))
            {
                return NotFound("Currencies file not found.");
            }

            var countries = System.IO.File.ReadAllText(countriesPath);
            return Content(countries, "application/json");
        }



    }
}

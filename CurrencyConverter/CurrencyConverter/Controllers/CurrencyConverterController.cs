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
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favorites = await _context.FavoriteCurrencyPairs
                    .Where(f => f.UserId == userId)
                    .ToListAsync();
                ViewBag.Favorites = favorites;
            }

            var currencies = await _exchangeRateService.GetCurrenciesAsync();
            ViewBag.Currencies = currencies;
            var historicalRates = await _exchangeRateService.GetHistoricalRatesAsync(DateTime.UtcNow);
            ViewBag.HistoricalRates = historicalRates;

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
                        var existingFavorite = await _context.FavoriteCurrencyPairs
                            .FirstOrDefaultAsync(f => f.UserId == userId &&
                                                      f.BaseCurrency == model.BaseCurrency &&
                                                      f.TargetCurrency == model.TargetCurrency);
                        if (existingFavorite != null)
                        {
                            return Json(new { success = true, convertedAmount = model.ConvertedAmount, errorMessage = "This currency pair is already saved as a favorite." });
                        }
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
        public async Task<IActionResult> HistoricalRates(DateTime date)
        {
            var rates = await _exchangeRateService.GetHistoricalRatesAsync(date);
            return Json(rates);
        }
    }
}

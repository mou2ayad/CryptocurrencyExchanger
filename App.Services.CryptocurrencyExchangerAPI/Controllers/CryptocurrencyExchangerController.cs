using App.Components.Utilities.JWT_Auth;
using App.Services.CryptocurrencyExchangerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptocurrencyExchanger.Controllers
{
    [ApiController]
    [Route("api/v1/cryptocurrency/[action]")]
    public class CryptocurrencyExchangerController : ControllerBase
    {       
        private readonly ICryptocurrencyExchangeProvider _exchangeProvider;

        public CryptocurrencyExchangerController(ICryptocurrencyExchangeProvider exchangeProvider)
        {
            _exchangeProvider = exchangeProvider;
        }
        [HttpGet("{symbol}")]        
        [JWTAuthorize("Quotes")]
        public async Task<ActionResult> QuotesAsync(string symbol)
        {
            var results = await _exchangeProvider.GetExchangeRateListAsync(symbol);
            return Ok(results);
        }


    }
}

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortnerApi.DAL;
using UrlShortnerApi.Models;
using UrlShortnerApi.Services;

namespace UrlShortnerApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("UrlShortnerPolicy")]
    public class ShortUrlController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;
        private readonly ILogger _logger;

        public ShortUrlController(IShortUrlService shortUrlService, ILogger<ShortUrlController> logger)
        {
            _shortUrlService = shortUrlService;
            _logger = logger;
        }

        
        [HttpGet("{shortUrl}", Name = "GetOriginalUrlAsync")]
        public async Task<IActionResult> GetOriginalUrlAsync(string shortUrl)
        {
            var originalUrl = await _shortUrlService.GetOriginalUrl(shortUrl);

            if (string.IsNullOrEmpty(originalUrl))
            {
                return NotFound();
            }

            return Ok(originalUrl);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShortUrlAsync([FromBody] string originalUrl)
        {
            try
            {
                var shortCode = await _shortUrlService.CreateShortUrl(originalUrl);
                return Content(shortCode);
            }
            catch (DocumentClientException ex)
            {
                _logger.LogError(ex, "Error occurred when creating short url for original url {originalUrl}", originalUrl);
                return Conflict("A resource with the specified id or name already exists");
            }
        }
    }
}

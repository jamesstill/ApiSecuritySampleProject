using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiSecuritySampleProject.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Here security is ignored. Anyone can get the weather forecast!
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Reader")]
        public async Task<IActionResult> Get(string id)
        {
            // some hypothetical GET method for an id that only readers can call
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "Writer")]
        public async Task<IActionResult> Post([FromBody]object o)
        {
            // some hypothetical POST method that only writers can call
            await Task.CompletedTask;
            return NoContent();
        }
    }
}

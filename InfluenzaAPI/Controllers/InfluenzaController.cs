using Microsoft.AspNetCore.Mvc;
using InfluenzaAPI.Models;

namespace InfluenzaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfluenzaController : ControllerBase
    {
        private readonly List<InfluenzaRecord> _allRecords;

        public InfluenzaController(List<InfluenzaRecord> allRecords)
        {
            _allRecords = allRecords ?? new List<InfluenzaRecord>();
        }

        /// <summary>
        /// Returns monthly aggregated influenza cases, optionally filtered by country.
        /// </summary>
        /// <param name="country">Country to filter by (optional).</param>
        /// <returns>List of monthly case totals.</returns>
        [HttpGet("monthly")]
        public IActionResult GetMonthlyFluData([FromQuery] string? country = null)
        {
            IEnumerable<InfluenzaRecord> query = _allRecords;

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(r =>
                    r.Country.Equals(country, StringComparison.OrdinalIgnoreCase));
            }

            var monthlyData = query
                .Where(r => r.Date != DateTime.MinValue)
                .GroupBy(r => new { r.Date.Year, r.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyFluSummaryExtended
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalCases = g.Sum(r => r.Cases),
                    TotalDeaths = g.Sum(r => r.Deaths),
                    EstimatedActive = (int)(g.Sum(r => r.Cases) * 0.2),
                    EstimatedRecovered = (int)(g.Sum(r => r.Cases) * 0.8)
                })
                .ToList();

            return Ok(monthlyData);
        }

        /// <summary>
        /// Basic health check endpoint.
        /// </summary>
        [HttpGet("ping")]
        public ActionResult<string> Ping() => Ok("pong");

        [HttpGet("summary")]
        public IActionResult GetFluSummary([FromQuery] string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return BadRequest("Country parameter is required.");
            }

            var countryRecords = _allRecords
                .Where(r => r.Country.Equals(country, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!countryRecords.Any())
            {
                return NotFound($"No records found for country: {country}");
            }

            int totalCases = countryRecords.Sum(r => r.Cases);
            int estimatedActive = (int)(totalCases * 0.2);     // 20% assumed active
            int estimatedRecovered = (int)(totalCases * 0.8);  // 80% assumed recovered
            int totalDeaths = countryRecords.Sum(r => r.Deaths); // Defaults to 0

            var summary = new FluSummary
            {
                Country = country,
                Cases = totalCases,
                Active = estimatedActive,
                Recovered = estimatedRecovered,
                Deaths = totalDeaths
            };

            return Ok(summary);
        }
    }
}

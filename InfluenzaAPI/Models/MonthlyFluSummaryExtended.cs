namespace InfluenzaAPI.Models
{
    public class MonthlyFluSummaryExtended
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalCases { get; set; }
        public int TotalDeaths { get; set; }
        public int EstimatedActive { get; set; }
        public int EstimatedRecovered { get; set; }
    }

}

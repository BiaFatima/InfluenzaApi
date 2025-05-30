namespace InfluenzaAPI.Models
{
    public class FluSummary
    {
        public string Country { get; set; }
        public int Cases { get; set; }
        public int Active { get; set; }
        public int Recovered { get; set; }
        public int Deaths { get; set; }
    }
}

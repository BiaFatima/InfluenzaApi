using CsvHelper.Configuration.Attributes;
using System;

namespace InfluenzaAPI.Models
{
    public class InfluenzaRecord
    {
        [Name("Country")]
        public string Country { get; set; }

        [Name("SDATE")]
        public string DateString { get; set; }

        [Name("ALL_INF")]
        public int Cases { get; set; }

        public int Deaths { get; set; } = 0;

        public DateTime Date
        {
            get
            {
                if (DateTime.TryParse(DateString, out var d))
                    return d;
                return DateTime.MinValue;
            }
        }
    }
}

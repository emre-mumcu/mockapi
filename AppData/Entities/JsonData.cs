using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPI.AppData.Entities
{
    public class JsonData
    {
        public required string Id { get; set; }
        public required string City { get; set; }
        public required string City_ascii { get; set; }
        public required string State_id { get; set; }
        public required string State_name { get; set; }
        public required string County_fips { get; set; }
        public required string County_name { get; set; }
        public required string Lat { get; set; }
        public required string Lng { get; set; }
        public required string Population { get; set; }
        public required string Density { get; set; }
        public required string Source { get; set; }
        public required string Military { get; set; }
        public required string Incorporated { get; set; }
        public required string Timezone { get; set; }
        public required string Ranking { get; set; }
        public required string Zips { get; set; }
    }
}
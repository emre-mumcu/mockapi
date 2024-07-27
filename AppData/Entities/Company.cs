using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPI.AppData.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? CatchPhrase { get; set; }
        public string? Bs { get; set; }
    }
}
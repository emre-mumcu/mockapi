using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPI.AppData.Entities
{
    public class State
    {
        public int Id { get; set; }
        public required string StateId { get; set; }
        public required string StateName { get; set; }
    }
}
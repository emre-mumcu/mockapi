using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPI.AppData.Entities
{
    public class ActionLog
    {
        public int Id { get; set; }
        public string? ConnectionId { get; set; }
        public string? TraceId { get; set; }
        public required string Scheme { get; set; }
        public required string Protocol { get; set; }
        public required string Host { get; set; }
        public required string Port { get; set; }
        public required string Path { get; set; }
        public required string Method { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public required string EventName { get; set; }
        public string? EventData { get; set; }
        public string? RouteData { get; set; }        
        public string? ModelState { get; set; }
        public string? Query { get; set; }
        public string? QueryString { get; set; }
        public required string LogTimestamp { get; set; }
        public required string RequestIp { get; set; }
    }
}
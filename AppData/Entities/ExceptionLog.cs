namespace MockAPI.AppData.Entities
{
    public class ExceptionLog
    {
        public int Id { get; set; }
        public required string TraceId { get; set; }
        public required string Source { get; set; }
        public required string Message { get; set; }        
        public string? InnerException { get; set; }
        public required string StackTrace { get; set; }
    }
}
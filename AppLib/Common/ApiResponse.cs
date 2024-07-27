namespace MockAPI.AppLib.Common
{
    public class ApiResponse<T> : ApiResponse
    {
        public new T? ResponseData { get; set; }
    }

    public class ApiResponse
    {
        public ApiResponse()
        {
            TraceId = App.Instance?._HttpContextAccessor?.HttpContext?.Items[Literals.TraceId]?.ToString() 
                ?? 
                Guid.NewGuid().ToString();
        }

        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; } = null!;
        public string? ResponseData { get; set; } = null;
        public string? Exception { get; set; } = null;
        public string? InnerException { get; set; } = null;
        public string? TraceId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public long ExecutionTime { get; set; }
    }

    public enum ApiResponseCodes
    {
        // Operation is completed.
        SUCCESS = 0,
        // Operation is NOT completed due to request.
        FAIL = 1,
        // Operation is NOT completed due to application error.
        EXCEPTION = 2
    }
}
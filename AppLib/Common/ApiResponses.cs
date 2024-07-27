namespace MockAPI.AppLib.Common;

public static class ApiResponses
{
    public static ApiResponse Success(string? msg = null) => new ApiResponse()
    {
        ResponseCode = (int)ApiResponseCodes.SUCCESS,
        ResponseMessage = msg ?? ApiResponseCodes.SUCCESS.ToString()
    };

    public static ApiResponse<T> Success<T>(T? data, long executionTime = 0, string? msg = null)
    {
        return new ApiResponse<T>()
        {
            ResponseCode = (int)ApiResponseCodes.SUCCESS,
            ResponseMessage = msg ?? ApiResponseCodes.SUCCESS.ToString(),
            ResponseData = data,
            ExecutionTime = executionTime
        };
    }

    public static ApiResponse Fail(string? msg = null)
    {
        return new ApiResponse()
        {
            ResponseCode = (int)ApiResponseCodes.FAIL,
            ResponseMessage = msg ?? ApiResponseCodes.FAIL.ToString(),
        };
    }

    public static ApiResponse Exception(Exception ex, string? msg = null)
    {
        return new ApiResponse()
        {
            ResponseCode = (int)ApiResponseCodes.EXCEPTION,
            ResponseMessage = msg ?? ApiResponseCodes.EXCEPTION.ToString(),
            Exception = ex?.Message,
            InnerException = ex?.InnerException?.Message
        };
    }
}
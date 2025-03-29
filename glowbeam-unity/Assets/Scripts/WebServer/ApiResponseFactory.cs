public static class ApiResponseFactory
{
    public static ApiResponse Success(string message, object data = null)
    {
        return new ApiResponse
        {
            success = true,
            message = message,
            data = data
        };
    }

    public static ApiResponse Error(string message, object data = null)
    {
        return new ApiResponse
        {
            success = false,
            message = message,
            data = data
        };
    }
}

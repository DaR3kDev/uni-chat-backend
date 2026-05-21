namespace uni_chat_backend.API.Responses;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Errors { get; set; }
}

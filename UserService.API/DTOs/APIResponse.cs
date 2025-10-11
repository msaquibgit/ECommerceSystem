namespace UserService.API.DTOs
{
    public class APIResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        public static APIResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new APIResponse<T> { Success = true, Data = data, Message = message };
        }

        public static APIResponse<T> FailResponse(string message, List<string>? errors = null, T? data = default)
        {
            return new APIResponse<T> { Success = false, Data = data, Message = message, Errors = errors };

        }
    }
}

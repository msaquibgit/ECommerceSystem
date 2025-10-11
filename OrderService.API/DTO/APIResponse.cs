namespace OrderService.API.DTO
{
    public class APIResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }


        public static APIResponse<T> SuccessResposne(T data, string? message)
        {
            return new APIResponse<T> { Success = true, Data = data, Message = message };   
        }
        public static APIResponse<T> FailResponse(string? message,List<string>? errors=null, T? data = default)
        {
            return new APIResponse<T> { Success = false, Data = data, Message = message, Errors = errors };         
        }
    }
}

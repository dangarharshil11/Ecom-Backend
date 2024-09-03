namespace CustomerService._4.Infrastructure.Models
{
    public class CommonResponse
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = "";
        public object? Data { get; set; }
    }
}

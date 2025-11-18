namespace DotBet.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }
        public static Result<T> Success(T value, string successMessage) => new Result<T> { IsSuccess = true, Value = value, Message = successMessage };
        public static Result<T> Failure(string errorMessage) => new Result<T> { IsSuccess = false, Message = errorMessage };
    }
}

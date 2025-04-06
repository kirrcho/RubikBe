namespace RubikCube.Common
{
    public class Result<T>
    {
        public T? Value { get; set; }

        public string? Error { get; set; }

        public bool IsSuccessful { get; set; }

        public static Result<T> BadResult(string error)
            => new Result<T>()
            {
                Error = error,
                IsSuccessful = false
            };

        public static Result<T> OkResult(T value)
            => new Result<T>()
            {
                Value = value,
                IsSuccessful = true
            };
    }
}
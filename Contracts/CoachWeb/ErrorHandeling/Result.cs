namespace Contracts.CoachWeb.ErrorHandeling
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; }
        public string? Error { get; }
        public ErrorType? ErrorCategory { get; }

        private Result(bool isSuccess, T? value, string? error, ErrorType? errorCategory)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            ErrorCategory = errorCategory;
        }

        public static Result<T> Success(T value) =>
            new Result<T>(true, value, null, null);

        public static Result<T> Failure(string error, ErrorType errorCategory = ErrorType.UserError) =>
            new Result<T>(false, default, error, errorCategory);
    }
}
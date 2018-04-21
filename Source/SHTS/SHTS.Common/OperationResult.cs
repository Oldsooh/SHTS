namespace Witbird.SHTS.Common
{
    public class OperationResult<T>
    {
        public OperationResult()
        {

        }

        public OperationResult(T result, string errorMessage, bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
            Result = result;
        }

        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public T Result { get; set; }
    }

    public class OperationResult : OperationResult<object>
    {
        public OperationResult()
        {
        }
    }
}

namespace Sauvio.Business.Exceptions
{
    public class ValidationException : BusinessException
    {
        public ValidationException(string message) : base(message) { }
    }
}

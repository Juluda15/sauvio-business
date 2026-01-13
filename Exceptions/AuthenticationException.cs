namespace Sauvio.Business.Exceptions
{
    public class AuthenticationException : BusinessException
    {
        public AuthenticationException(string message) : base(message) { }
    }
}

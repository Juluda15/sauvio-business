namespace Sauvio.Business.Exceptions
{
    public class NotFoundException : BusinessException
    {
        public NotFoundException(string entity, object key)
            : base($"{entity} with identifier '{key}' was not found.") { }
    }
}

namespace VerificationPortal.Exceptions
{
    public class VerificationDataNotFoundException : Exception
    {
        public VerificationDataNotFoundException(string entityName)
            : base($"{entityName} record not found.")
        {
            EntityName = entityName;
        }

        public string EntityName { get; }
    }
}
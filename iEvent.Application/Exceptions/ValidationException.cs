namespace iEvent.Application.Exceptions
{
    public class ValidationException : AppException
    {
        public IReadOnlyCollection<string> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = Array.Empty<string>();
        }

        public ValidationException(string message, IEnumerable<string> errors) : base(message)
        {
            Errors = errors.ToArray();
        }
    }
}
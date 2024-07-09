namespace DokWokApi.Exceptions
{
    public class UserException : Exception
    {
        public UserException()
        {
        }

        public UserException(string paramName, string message)
            : base($"{message}; Parameter name: {paramName}")
        {
        }

        public UserException(string message)
            : base(message)
        {
        }

        public UserException(string paramName, string message, Exception inner)
            : base($"{message}; Parameter name: {paramName}", inner)
        {
        }

        public UserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

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

        public UserException(string paramName, string message, Exception inner)
            : base($"{message}; Parameter name: {paramName}", inner)
        {
        }
    }
}

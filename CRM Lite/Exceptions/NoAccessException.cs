using System;

namespace CRM.API.Exceptions
{
    public class NoAccessException : Exception
    {
        public NoAccessException()
        {
        }

        public NoAccessException(string message)
            : base(message)
        {
        }

        public NoAccessException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

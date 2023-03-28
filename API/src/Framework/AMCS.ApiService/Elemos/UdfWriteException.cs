namespace AMCS.ApiService.Elemos
{
    using System;

    public class UdfWriteException : Exception
    {
        public UdfWriteException()
        {
        }

        public UdfWriteException(string message)
            : base(message)
        {
        }

        public UdfWriteException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
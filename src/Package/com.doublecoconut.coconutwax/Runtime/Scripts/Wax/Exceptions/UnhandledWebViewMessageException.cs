using System;

namespace Wax.Exceptions
{
    public class UnhandledWebViewMessageException:Exception
    {
        public UnhandledWebViewMessageException(string message) : base(message)
        {
        }
    }
}
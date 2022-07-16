using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Exceptions
{
    public class InvalidFileSizeException : Exception
    {
        public InvalidFileSizeException()
        {
        }

        public InvalidFileSizeException(string message) : base(message)
        {
        }

        public InvalidFileSizeException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}

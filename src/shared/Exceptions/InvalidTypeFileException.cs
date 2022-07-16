using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Exceptions
{
    public class InvalidTypeFileException : Exception
    {
        public InvalidTypeFileException()
        {
        }

        public InvalidTypeFileException(string message) : base(message)
        {
        }

        public InvalidTypeFileException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}

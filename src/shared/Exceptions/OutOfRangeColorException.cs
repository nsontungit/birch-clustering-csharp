using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Exceptions
{
    public class OutOfRangeColorException : Exception
    {
        public OutOfRangeColorException()
        {
        }

        public OutOfRangeColorException(string message) : base(message)
        {
        }

        public OutOfRangeColorException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}

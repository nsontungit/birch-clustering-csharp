using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Exceptions
{
    public class NotFoundDateException : Exception
    {
        public NotFoundDateException()
        {
        }

        public NotFoundDateException(string message) : base(message)
        {
        }

        public NotFoundDateException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}

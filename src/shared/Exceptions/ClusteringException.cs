using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Exceptions
{
    public class ClusteringException : Exception
    {
        public ClusteringException()
        {
        }

        public ClusteringException(string message) : base(message)
        {
        }

        public ClusteringException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAuthorisation.Errors
{
    internal class UserException : ArgumentException
    {
        public UserException(string paramName, string message) : base(message, paramName) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahApps.Metro
{
    internal class InvalidContentException : Exception
    {
        public InvalidContentException(string message)
    : base(message)
        {
        }
    }
}

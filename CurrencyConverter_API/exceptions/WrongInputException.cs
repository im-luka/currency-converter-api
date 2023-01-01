using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_API.exceptions
{
    class WrongInputException : Exception
    {
        public WrongInputException() { }

        public WrongInputException(string message) : base(message) { }

        public WrongInputException(string message, Exception exception) : base(message, exception) { }

    }
}

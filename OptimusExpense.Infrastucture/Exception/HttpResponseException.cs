using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Infrastucture.Exception
{
    public class HttpResponseException : System.Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }
    }
}

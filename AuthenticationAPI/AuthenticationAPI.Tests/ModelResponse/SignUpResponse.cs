using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationAPI.Tests.ModelResponse
{
    internal class SignUpResponse
    {
        public int StatusCode { get; set; }
        public object Data { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}

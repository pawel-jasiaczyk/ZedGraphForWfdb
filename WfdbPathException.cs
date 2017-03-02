using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedGraphForWfdb
{
    class WfdbPathException : ApplicationException
    {
        private string message;
        
        public WfdbPathException(string message)
        {
            this.message = message;
        }

        public override string Message { get { return this.message; } }
    }
}

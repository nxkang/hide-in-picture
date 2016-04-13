using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoHidden.ViewModel.VMException
{
    class FileFormatException : ApplicationException
    {
        public FileFormatException()
        {

        }

        public FileFormatException(string msg) : base(msg)
        {

        }

        public FileFormatException(string msg, Exception inner) : base(msg, inner)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoHidden.Service.ServiceException
{
    public class PasswordWrongException : ApplicationException
    {
        public PasswordWrongException()
        {

        }

        public PasswordWrongException(string msg) : base(msg)
        {

        }

        public PasswordWrongException(string msg, Exception inner) : base(msg, inner)
        {

        }
    }
}

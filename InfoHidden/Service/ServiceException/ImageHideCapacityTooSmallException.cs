using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoHidden.Service.ServiceException
{
    public class ImageHideCapacityTooSmallException : ApplicationException
    {
        public ImageHideCapacityTooSmallException(){ }

        public ImageHideCapacityTooSmallException(string message)
            :base(message) { }

        public ImageHideCapacityTooSmallException(string message, Exception inner)
            :base(message, inner) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoHidden.Model
{
    [Serializable]
    public class DeHideOption
    {
        public string Password { get; set; }

        public string FilePath { get; set; }

        public string EncryptionAlg { get; set; }

        public static bool IsValid(DeHideOption deHideOption)
        {
            if (string.IsNullOrEmpty(deHideOption.Password)
                || string.IsNullOrEmpty(deHideOption.FilePath))
                return false;

            return true;
        }
    }
}

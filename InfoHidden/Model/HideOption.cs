using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoHidden.Model
{
    [Serializable]
    public class HideOption
    {
        public string Password { get; set; }

        public string EncryptionAlg { get; set; }

        public string FilePath { get; set; }


        public static bool IsValid(HideOption hideOption)
        {
            if (string.IsNullOrEmpty(hideOption.Password)
                || string.IsNullOrEmpty(hideOption.EncryptionAlg)
                || string.IsNullOrEmpty(hideOption.FilePath))
                return false;

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace InfoHidden.Model
{
    public class PictureInfo
    {

        public string FilePath { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public string BitsPerPixel { get; set; }

        public string MaxCapacity { get; set; }

        public static PictureInfo CreatePictureInfo(Bitmap image)
        {
            PictureInfo pictureInfo = new PictureInfo();

            pictureInfo.Width = image.Width.ToString();
            pictureInfo.Height = image.Height.ToString();
            pictureInfo.BitsPerPixel = 24.ToString();
            pictureInfo.MaxCapacity = ( (image.Width * image.Height - 32)/8/1024 ).ToString(); // KB

            return pictureInfo;
        }
    }
}

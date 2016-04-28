using InfoHidden.Model;
using InfoHidden.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace InfoHidden.ViewModel
{
    public class PictureInfoViewModel
    {

        public PictureInfoViewModel()
        {
            
        }

        #region Fileds

        private static PictureInfo pictureInfo;

        #endregion


        #region Properties

        public string FilePath => pictureInfo.FilePath;

        public string Dimension => pictureInfo.Width + " X " + pictureInfo.Height;

        public string BitsPerPixel => pictureInfo.BitsPerPixel;

        public string MaxCapacity => pictureInfo.MaxCapacity +　"KB";

        #endregion

        public void DoUpdate(Window parentWin)
        {
            pictureInfo = (PictureInfo)Application.Current.Properties["pictureInfo"];
            Application.Current.Properties["pictureInfo"] = null;


            Window win = new PictureInfoView();
            win.Owner = parentWin;

            win.ShowDialog();
        }
    }
}

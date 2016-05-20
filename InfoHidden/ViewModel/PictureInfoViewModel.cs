namespace InfoHidden.ViewModel
{
    using System.Windows;

    using InfoHidden.Model;
    using InfoHidden.View;

    public class PictureInfoViewModel
    {
        #region Static fields

        private static PictureInfo pictureInfo;

        #endregion

        #region Constructors and destructors

        public PictureInfoViewModel()
        {
        }

        #endregion

        #region Public properties

        public string BitsPerPixel => pictureInfo.BitsPerPixel;

        public string Dimension => pictureInfo.Width + " X " + pictureInfo.Height;

        public string FilePath => pictureInfo.FilePath;

        public string MaxCapacity => pictureInfo.MaxCapacity + "KB";

        #endregion

        #region Public methods

        public void DoUpdate(Window parentWin)
        {
            pictureInfo = (PictureInfo)Application.Current.Properties["pictureInfo"];
            Application.Current.Properties["pictureInfo"] = null;

            Window win = new PictureInfoView();
            win.Owner = parentWin;

            win.ShowDialog();
        }

        #endregion
    }
}
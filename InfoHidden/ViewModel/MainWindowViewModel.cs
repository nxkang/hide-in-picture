namespace InfoHidden.ViewModel
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using InfoHidden.Model;
    using InfoHidden.Service;
    using InfoHidden.Service.Encryption;
    using InfoHidden.Service.Encryption.Impl;
    using InfoHidden.Service.ServiceException;
    using InfoHidden.Utility;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;

    internal class MainWindowViewModel : NotificationObject
    {
        #region Fields

        private BitmapImage _coverImage;

        private Bitmap _coverImageBitmapCache;

        private BitmapImage _hiddenImage;

        private Bitmap _hiddenImageBitmapCache;

        private string _imageFilePath;

        #endregion

        #region Constructors and destructors

        public MainWindowViewModel()
        {
            this.OpenFileCommand = new DelegateCommand<object>(this.ExecuteOpenFile, this.CanExecuteOpenFile);
            this.SaveAsCommand = new DelegateCommand(this.ExecuteSaveAs, this.CanExecuteSaveAs);
            this.ClosePictureCommand = new DelegateCommand(this.ExecuteClosePicture, this.CanExecuteClosePicture);

            this.HideFileCommand = new DelegateCommand<object>(this.ExecuteHideFile, this.CanExecuteHideFile);
            this.RetrieveFileCommand = new DelegateCommand<object>(
                this.ExecuteRetrieveFile, 
                this.CanExecuteRetrieveFile);
            this.EraseFileCommand = new DelegateCommand(this.ExecuteEraseFile, this.CanExecuteEraseFile);

            this.PictureInfoCommand = new DelegateCommand<object>(this.ExecutePictureInfo, this.CanExecutePictureInfo);
            this.PictureZoomCommand = new DelegateCommand<object>(this.ExecutePictureZoom, this.CanExecutePictureZoom);

            this.SwitchLangCommand = new DelegateCommand<object>(this.ExecuteSwitchLang, this.CanExecuteSwitchLang);
        }

        #endregion

        #region Public properties

        public DelegateCommand ClosePictureCommand { get; set; }

        public BitmapImage CoverImage
        {
            get
            {
                return this._coverImage;
            }

            set
            {
                this._coverImage = value;
                this.RaisePropertyChanged(nameof(this.CoverImage));

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public DelegateCommand EraseFileCommand { get; set; }

        public BitmapImage HiddenImage
        {
            get
            {
                return this._hiddenImage;
            }

            set
            {
                this._hiddenImage = value;
                this.RaisePropertyChanged(nameof(this.HiddenImage));

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public DelegateCommand<object> HideFileCommand { get; set; }

        public string ImageFilePath
        {
            get
            {
                return this._imageFilePath;
            }

            set
            {
                this._imageFilePath = value;
                this.RaisePropertyChanged(nameof(this.ImageFilePath));

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public DelegateCommand<object> OpenFileCommand { get; set; }

        public DelegateCommand<object> PictureInfoCommand { get; set; }

        public DelegateCommand<object> PictureZoomCommand { get; set; }

        // public DelegateCommand<object> HideFileCommand { get; set; }
        public DelegateCommand<object> RetrieveFileCommand { get; set; }

        public DelegateCommand SaveAsCommand { get; set; }

        public DelegateCommand<object> SwitchLangCommand { get; set; }

        #endregion

        #region Public methods

        public bool CanExecuteClosePicture()
        {
            return this.CoverImage != null || this.HiddenImage != null;
        }

        public bool CanExecuteEraseFile()
        {
            return this.HiddenImage != null;
        }

        public bool CanExecuteHideFile(object args)
        {
            return this.CoverImage != null;
        }

        public bool CanExecuteOpenFile(object args)
        {
            return true;
        }

        public bool CanExecutePictureInfo(object sender)
        {
            return this.CoverImage != null;
        }

        public bool CanExecutePictureZoom(object args)
        {
            return this.CoverImage != null || this.HiddenImage != null;
        }

        public bool CanExecuteRetrieveFile(object args)
        {
            return this.HiddenImage != null;
        }

        public bool CanExecuteSaveAs()
        {
            return this.HiddenImage != null;
        }

        public bool CanExecuteSwitchLang(object args)
        {
            return true;
        }

        public void ExecuteClosePicture()
        {
            var isConfirm = this.ShowMessageBoxResource("ConfirmClose", "Hint", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (isConfirm == false)
            {
                return;
            }

            this.CoverImage = null;
            this._coverImageBitmapCache = null;
            this.HiddenImage = null;
            this._hiddenImageBitmapCache = null;

            this.ImageFilePath = null;
        }

        public void ExecuteEraseFile()
        {
            HideLSB.Erase(ref this._hiddenImageBitmapCache);
            this.HiddenImage = FileTransform.Bitmap2BitmapImage(this._hiddenImageBitmapCache);

            if (File.Exists(this.ImageFilePath))
            {
                File.Delete(this.ImageFilePath);
            }

            this._hiddenImageBitmapCache.Save(this.ImageFilePath);

            this.ShowMessageBoxResource("RetrieveDone", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ExecuteHideFile(object args)
        {
            try
            {
                var hideOption = this.GetHideOptionFromView((Window)args);

                if (hideOption == null)
                {
                    return;
                }

                var fileBytesToHide = FileTransform.File2ByteArray(hideOption.FilePath);
                var zipedCoverImageBytes = Zip.Compress(fileBytesToHide);

                IEncryption encryptor = EncryptionFactory.CreateEncryption(hideOption.EncryptionAlg);

                var ciphertext = encryptor.Encrypt(zipedCoverImageBytes, this.StrPassword2UintArr(hideOption.Password));

                var tmpBitmapCacheToHide =
                    this._coverImageBitmapCache.Clone(
                        new Rectangle(0, 0, this._coverImageBitmapCache.Width, this._coverImageBitmapCache.Height), 
                        this._coverImageBitmapCache.PixelFormat);

                HideLSB.Hide(ref tmpBitmapCacheToHide, ciphertext);

                this._hiddenImageBitmapCache = tmpBitmapCacheToHide;
                this.HiddenImage = FileTransform.Bitmap2BitmapImage(this._hiddenImageBitmapCache);
            }
            catch (ImageHideCapacityTooSmallException)
            {
                this.ShowMessageBoxResource("HidingCapacityNotEnough", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException)
            {
                this.ShowMessageBoxResource("FileOpenError", "Error");
            }
            catch (Exception)
            {
                this.ShowMessageBoxResource("UnknownError", "Error");
            }
        }

        public void ExecuteOpenFile(object args)
        {
            try
            {
                var fileTypesPattern = "bmp file (*.bmp)|*.bmp|All files (*.*)|*.*";
                var defaultExt = "bmp";
                var imageUri = GetFilePathFromFileDialog.GetFilePathFromOpenFileDialog(fileTypesPattern, defaultExt);

                if (string.IsNullOrEmpty(imageUri))
                {
                    return;
                }

                var bitmapImg = FileTransform.ImageUri2BitmapImage(imageUri);
                var bitmapCache = FileTransform.BitmapImage2Bitmap(bitmapImg);

                var openCmdPara = (string)args;
                if ("Cover".Equals(openCmdPara))
                {
                    this.CoverImage = bitmapImg;
                    this._coverImageBitmapCache = bitmapCache;
                    this.HiddenImage = null;
                    this._hiddenImageBitmapCache = null;
                }
                else if ("Hidden".Equals(openCmdPara))
                {
                    this.HiddenImage = bitmapImg;
                    this._hiddenImageBitmapCache = bitmapCache;
                    this.CoverImage = null;
                    this._coverImageBitmapCache = null;
                }

                this.ImageFilePath = imageUri;
            }
            catch (FileFormatException)
            {
                this.ShowMessageBoxResource("MustBeBmpFile", "Hint");
            }
            catch (Exception)
            {
                this.ShowMessageBoxResource("MustBeBmpFile", "Hint");
            }
        }

        public void ExecutePictureInfo(object sender)
        {
            Application.Current.Properties["pictureInfo"] = PictureInfo.CreatePictureInfo(this._coverImageBitmapCache);
            ((PictureInfo)Application.Current.Properties["pictureInfo"]).FilePath = this.ImageFilePath;
            new PictureInfoViewModel().DoUpdate(sender as Window);
            Application.Current.Properties["pictureInfo"] = null;
        }

        public void ExecutePictureZoom(object args)
        {
            var mainWindow = Application.Current.MainWindow;

            var tfs = (TransformGroup)mainWindow.FindResource("ImageTransformResource");
            var scaleTf = (ScaleTransform)tfs.Children[0];

            var zoomPara = double.Parse((string)args) / 100;

            scaleTf.ScaleX = zoomPara;
            scaleTf.ScaleY = zoomPara;
        }

        public void ExecuteRetrieveFile(object args)
        {
            try
            {
                var deHideOption = this.GetDeHideOptionFromView((Window)args);

                if (deHideOption == null)
                {
                    return;
                }

                var ciphertext = HideLSB.DeHide(this._hiddenImageBitmapCache);

                IEncryption encryptor = EncryptionFactory.CreateEncryption(deHideOption.EncryptionAlg);
                var plaintext = encryptor.Decrypt(ciphertext, this.StrPassword2UintArr(deHideOption.Password));
                var deZipdata = Zip.Decompress(plaintext);

                if (File.Exists(deHideOption.FilePath))
                {
                    File.Delete(deHideOption.FilePath);
                }

                FileTransform.ByteArray2File(deHideOption.FilePath, deZipdata);

                this.ShowMessageBoxResource("RetrieveDone", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (PasswordWrongException)
            {
                this.ShowMessageBoxResource("PasswordWrong", "Error");
            }
            catch (Exception)
            {
                this.ShowMessageBoxResource("PasswordWrong", "Error");
            }
        }

        public void ExecuteSaveAs()
        {
            var fileTypesPattern = "bmp file (*.bmp)|*.bmp|All files (*.*)|*.*";
            var defaultExt = "bmp";
            var savePath = GetFilePathFromFileDialog.GetFilePahtFromSaveFileDialog(fileTypesPattern, defaultExt);

            if (savePath.Equals(string.Empty))
            {
                return;
            }

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            this._hiddenImageBitmapCache.Save(savePath);
        }

        public void ExecuteSwitchLang(object args)
        {
            string lang = args as string;

            if ("zh-CN".Equals(lang))
            {
                App.Culture = "zh-CN";
            }
            else
            {
                App.Culture = "en-US";
            }

            App.UpdateCulture();

            // MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("MsgSuccess").ToString()));
        }

        #endregion

        #region Other methods

        private DeHideOption GetDeHideOptionFromView(Window parentWin)
        {
            var deHideOptionViewModel = new DeHideOptionViewModel();
            deHideOptionViewModel.DoUpate(parentWin);
            var deHideOption = (DeHideOption)Application.Current.Properties["deHideOption"];
            Application.Current.Properties["deHideOption"] = null;

            return deHideOption;
        }

        private HideOption GetHideOptionFromView(Window parentWin)
        {
            var hovm = new HideOptionViewModel();
            hovm.DoUpdate(parentWin);
            var hideOption = (HideOption)Application.Current.Properties["hideOption"];
            Application.Current.Properties["hideOption"] = null;

            return hideOption;
        }

        private void RaiseAllCommandsCanExecuteChanged()
        {
            this.OpenFileCommand.RaiseCanExecuteChanged();
            this.ClosePictureCommand.RaiseCanExecuteChanged();
            this.SaveAsCommand.RaiseCanExecuteChanged();

            this.HideFileCommand.RaiseCanExecuteChanged();
            this.RetrieveFileCommand.RaiseCanExecuteChanged();

            this.EraseFileCommand.RaiseCanExecuteChanged();
            this.PictureInfoCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="msgBoxCaption">标题栏标题</param>
        /// <param name="msgBoxBtn">按钮</param>
        /// <param name="msgBoxImg">图标</param>
        /// <returns></returns>
        private bool ShowMessageBox(
            string msg, 
            string msgBoxCaption = "警告", 
            MessageBoxButton msgBoxBtn = MessageBoxButton.OK, 
            MessageBoxImage msgBoxImg = MessageBoxImage.Warning)
        {
            var isConfirm = MessageBox.Show(msg, msgBoxCaption, msgBoxBtn, msgBoxImg);
            if (isConfirm == MessageBoxResult.Yes || isConfirm == MessageBoxResult.OK)
            {
                return true;
            }

            return false;
        }

        private bool ShowMessageBoxResource(
            string msgKey,
            string msgBoxCaptionKey,
            MessageBoxButton msgBoxBtn = MessageBoxButton.OK,
            MessageBoxImage msgBoxImg = MessageBoxImage.Warning)
        {
            string msg = Application.Current.FindResource(msgKey) as string;
            string msgBoxCaption = Application.Current.FindResource(msgBoxCaptionKey) as string;

            var isConfirm = MessageBox.Show(msg, msgBoxCaption, msgBoxBtn, msgBoxImg);
            if (isConfirm == MessageBoxResult.Yes || isConfirm == MessageBoxResult.OK)
            {
                return true;
            }

            return false;
        }

        private uint[] StrPassword2UintArr(string strPassword)
        {
            var ret = new uint[strPassword.Length / 4];

            for (var i = 0; i < strPassword.Length; i += 4)
            {
                var idx = i / 4;
                ret[idx] = strPassword[idx + 3];
                ret[idx] |= (uint)(strPassword[idx + 2] << 8);
                ret[idx] |= (uint)(strPassword[idx + 1] << 16);
                ret[idx] |= (uint)(strPassword[idx] << 24);
            }

            return ret;
        }

        #endregion
    }
}
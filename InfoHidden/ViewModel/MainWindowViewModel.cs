using System;
using System.Drawing;
using System.Globalization;
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

namespace InfoHidden.ViewModel
{
    internal class MainWindowViewModel : NotificationObject
    {
        #region cntr

        public MainWindowViewModel()
        {
            OpenFileCommand = new DelegateCommand<object>(ExecuteOpenFile, CanExecuteOpenFile);
            SaveAsCommand = new DelegateCommand(ExecuteSaveAs, CanExecuteSaveAs);
            ClosePictureCommand = new DelegateCommand(ExecuteClosePicture, CanExecuteClosePicture);

            HideFileCommand = new DelegateCommand<object>(ExecuteHideFile, CanExecuteHideFile);
            RetrieveFileCommand = new DelegateCommand<object>(ExecuteRetrieveFile, CanExecuteRetrieveFile);
            EraseFileCommand = new DelegateCommand(ExecuteEraseFile, CanExecuteEraseFile);

            PictureInfoCommand = new DelegateCommand<object>(ExecutePictureInfo, CanExecutePictureInfo);
            PictureZoomCommand = new DelegateCommand<object>(ExecutePictureZoom, CanExecutePictureZoom);

            SwitchLangCommand = new DelegateCommand<object>(this.ExecuteSwitchLang, this.CanExecuteSwitchLang);
        }

        #endregion




        /// <summary>
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="msgBoxCaption">标题栏标题</param>
        /// <param name="msgBoxBtn">按钮</param>
        /// <param name="msgBoxImg">图标</param>
        /// <returns></returns>
        private bool ShowMessageBox(string msg,
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

        private HideOption GetHideOptionFromView(Window parentWin)
        {
            var hovm = new HideOptionViewModel();
            hovm.DoUpdate(parentWin);
            var hideOption = (HideOption) Application.Current.Properties["hideOption"];
            Application.Current.Properties["hideOption"] = null;

            return hideOption;
        }

        private DeHideOption GetDeHideOptionFromView(Window parentWin)
        {
            var deHideOptionViewModel = new DeHideOptionViewModel();
            deHideOptionViewModel.DoUpate(parentWin);
            var deHideOption = (DeHideOption) Application.Current.Properties["deHideOption"];
            Application.Current.Properties["deHideOption"] = null;

            return deHideOption;
        }

        private void RaiseAllCommandsCanExecuteChanged()
        {
            OpenFileCommand.RaiseCanExecuteChanged();
            ClosePictureCommand.RaiseCanExecuteChanged();
            SaveAsCommand.RaiseCanExecuteChanged();

            HideFileCommand.RaiseCanExecuteChanged();
            RetrieveFileCommand.RaiseCanExecuteChanged();

            EraseFileCommand.RaiseCanExecuteChanged();
            PictureInfoCommand.RaiseCanExecuteChanged();
        }

        private uint[] StrPassword2UintArr(string strPassword)
        {
            var ret = new uint[strPassword.Length/4];

            for (var i = 0; i < strPassword.Length; i += 4)
            {
                var idx = i/4;
                ret[idx] = strPassword[idx + 3];
                ret[idx] |= (uint) (strPassword[idx + 2] << 8);
                ret[idx] |= (uint) (strPassword[idx + 1] << 16);
                ret[idx] |= (uint) (strPassword[idx] << 24);
            }

            return ret;
        }



        #region  Commands Properties

        public DelegateCommand<object> OpenFileCommand { get; set; }

        public DelegateCommand ClosePictureCommand { get; set; }

        public DelegateCommand SaveAsCommand { get; set; }

        public DelegateCommand<object> HideFileCommand { get; set; }

        public DelegateCommand<object> RetrieveFileCommand { get; set; }

        public DelegateCommand EraseFileCommand { get; set; }

        public DelegateCommand<object> PictureInfoCommand { get; set; }

        public DelegateCommand<object> PictureZoomCommand { get; set; }

        public DelegateCommand<object> SwitchLangCommand { get; set; }

        #endregion

        #region Properites

        private BitmapImage _coverImage;

        public BitmapImage CoverImage
        {
            get { return _coverImage; }
            set
            {
                _coverImage = value;
                RaisePropertyChanged(nameof(CoverImage));

                RaiseAllCommandsCanExecuteChanged();
            }
        }

        private BitmapImage _hiddenImage;

        public BitmapImage HiddenImage
        {
            get { return _hiddenImage; }
            set
            {
                _hiddenImage = value;
                RaisePropertyChanged(nameof(HiddenImage));

                RaiseAllCommandsCanExecuteChanged();
            }
        }


        private Bitmap _coverImageBitmapCache;

        private Bitmap _hiddenImageBitmapCache;


        private string _imageFilePath;

        public string ImageFilePath
        {
            get { return _imageFilePath; }
            set
            {
                _imageFilePath = value;
                RaisePropertyChanged(nameof(ImageFilePath));

                RaiseAllCommandsCanExecuteChanged();
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                _isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));

                RaiseAllCommandsCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        public bool CanExecuteSwitchLang(object args)
        {
            return true;
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
     //       MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("MsgSuccess").ToString()));
        }


        public bool CanExecutePictureZoom(object args)
        {
            return CoverImage != null || HiddenImage != null;
        }

        public void ExecutePictureZoom(object args)
        {
            var mainWindow = Application.Current.MainWindow;

            var tfs = (TransformGroup) mainWindow.FindResource("ImageTransformResource");
            var scaleTf = (ScaleTransform) tfs.Children[0];

            var zoomPara = double.Parse((string) args)/100;

            scaleTf.ScaleX = zoomPara;
            scaleTf.ScaleY = zoomPara;
        }


        public bool CanExecutePictureInfo(object sender)
        {
            return CoverImage != null;
        }

        public void ExecutePictureInfo(object sender)
        {
            Application.Current.Properties["pictureInfo"] = PictureInfo.CreatePictureInfo(_coverImageBitmapCache);
            ((PictureInfo) Application.Current.Properties["pictureInfo"]).FilePath = ImageFilePath;
            new PictureInfoViewModel().DoUpdate(sender as Window);
            Application.Current.Properties["pictureInfo"] = null;
        }


        public bool CanExecuteEraseFile()
        {
            return HiddenImage != null;
        }

        public void ExecuteEraseFile()
        {
            IsBusy = true;

            HideLSB.Erase(ref _hiddenImageBitmapCache);
            HiddenImage = FileTransform.Bitmap2BitmapImage(_hiddenImageBitmapCache);

            IsBusy = false;

            ShowMessageBox("擦除已完成.", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public bool CanExecuteSaveAs()
        {
            return HiddenImage != null;
        }

        public void ExecuteSaveAs()
        {
            var fileTypesPattern = "bmp file (*.bmp)|*.bmp|All files (*.*)|*.*";
            var defaultExt = "bmp";
            var savePath = GetFilePathFromFileDialog.getFilePahtFromSaveFileDialog(fileTypesPattern, defaultExt);

            if(savePath.Equals(string.Empty))
                return;

            if (File.Exists(savePath))
                File.Delete(savePath);
            _hiddenImageBitmapCache.Save(savePath);
        }


        public bool CanExecuteOpenFile(object args)
        {
            return true;
        }

        public void ExecuteOpenFile(object args)
        {
            try
            {
                var fileTypesPattern = "bmp file (*.bmp)|*.bmp|All files (*.*)|*.*";
                var defaultExt = "bmp";
                var imageUri = GetFilePathFromFileDialog.getFilePathFromOpenFileDialog(fileTypesPattern, defaultExt);

                if (string.IsNullOrEmpty(imageUri))
                    return;

                var bitmapImg = FileTransform.ImageUri2BitmapImage(imageUri);
                var bitmapCache = FileTransform.BitmapImage2Bitmap(bitmapImg);

                var openCmdPara = (string) args;
                if ("Cover".Equals(openCmdPara))
                {
                    CoverImage = bitmapImg;
                    _coverImageBitmapCache = bitmapCache;
                    HiddenImage = null;
                    _hiddenImageBitmapCache = null;
                }
                else if ("Hidden".Equals(openCmdPara))
                {
                    HiddenImage = bitmapImg;
                    _hiddenImageBitmapCache = bitmapCache;
                    CoverImage = null;
                    _coverImageBitmapCache = null;
                }

                ImageFilePath = imageUri;
            }
            catch (FileFormatException)
            {
                ShowMessageBox("请打开bmp文件.");
            }
            catch (Exception)
            {
                ShowMessageBox("请打开bmp文件.");
            }
        }


        public bool CanExecuteHideFile(object args)
        {
            return CoverImage != null;
        }

        public void ExecuteHideFile(object args)
        {
            try
            {
                var hideOption = GetHideOptionFromView((Window) args);

                if (hideOption == null)
                    return;

                var fileBytesToHide = FileTransform.File2ByteArray(hideOption.FilePath);
                var zipedCoverImageBytes = Zip.Compress(fileBytesToHide);

                IEncryption encryptor = EncryptionFactory.CreateEncryption(hideOption.EncryptionAlg);

                var ciphertext = encryptor.Encrypt(zipedCoverImageBytes, StrPassword2UintArr(hideOption.Password));

                var tmpBitmapCacheToHide =
                    _coverImageBitmapCache.Clone(
                        new Rectangle(0, 0, _coverImageBitmapCache.Width, _coverImageBitmapCache.Height),
                        _coverImageBitmapCache.PixelFormat);

                HideLSB.Hide(ref tmpBitmapCacheToHide, ciphertext);

                _hiddenImageBitmapCache = tmpBitmapCacheToHide;
                HiddenImage = FileTransform.Bitmap2BitmapImage(_hiddenImageBitmapCache);


                
            }
            catch (ImageHideCapacityTooSmallException)
            {
                ShowMessageBox("该图片隐藏容量不足", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException)
            {
                ShowMessageBox("文件无法正常打开", "错误");
            }
        }


        public bool CanExecuteRetrieveFile(object args)
        {
            return HiddenImage != null;
        }

        public void ExecuteRetrieveFile(object args)
        {
            try
            {
                var deHideOption = GetDeHideOptionFromView((Window) args);

                if (deHideOption == null)
                    return;

                IsBusy = true;

                var ciphertext = HideLSB.DeHide(_hiddenImageBitmapCache);

                IEncryption encryptor = EncryptionFactory.CreateEncryption(deHideOption.EncryptionAlg);
                var plaintext = encryptor.Decrypt(ciphertext, StrPassword2UintArr(deHideOption.Password));
                var deZipdata = Zip.Decompress(plaintext);

                if (File.Exists(deHideOption.FilePath))
                    File.Delete(deHideOption.FilePath);
                FileTransform.ByteArray2File(deHideOption.FilePath, deZipdata);

                IsBusy = false;

                ShowMessageBox("Retrieve file is done.", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (PasswordWrongException)
            {
                ShowMessageBox("Password is wrong.");
            }
            catch (Exception)
            {
                ShowMessageBox("Password is wrong.");
            }
        }


        public bool CanExecuteClosePicture()
        {
            return CoverImage != null || HiddenImage != null;
        }

        public void ExecuteClosePicture()
        {
            var isConfirm = ShowMessageBox("确定关闭？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (isConfirm == false)
                return;

            CoverImage = null;
            _coverImageBitmapCache = null;
            HiddenImage = null;
            _hiddenImageBitmapCache = null;

            ImageFilePath = null;
        }

        #endregion
    }
}
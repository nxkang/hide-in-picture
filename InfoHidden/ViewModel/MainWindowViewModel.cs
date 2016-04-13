using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using InfoHidden.Model;
using InfoHidden.View;
using InfoHidden.Utility;
using InfoHidden.Service;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using InfoHidden.Service.ServiceException;
using System.Drawing.Imaging;
using System.Windows.Interop;
using System.Diagnostics;

namespace InfoHidden.ViewModel
{
    class MainWindowViewModel : NotificationObject
    {

        #region cntr

        public MainWindowViewModel()
        {
            this.OpenFileCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteOpenFile), new Func<object, bool>(this.CanExecuteOpenFile));
            this.SaveAsCommand = new DelegateCommand(new Action(this.ExecuteSaveAs), new Func<bool>(this.CanExecuteSaveAs));
            this.ClosePictureCommand = new DelegateCommand(new Action(this.ExecuteClosePicture), new Func<bool>(this.CanExecuteClosePicture));

            this.HideFileCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteHideFile), new Func<object, bool>(this.CanExecuteHideFile));
            this.RetrieveFileCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteRetrieveFile), new Func<object, bool>(this.CanExecuteRetrieveFile));
            this.EraseFileCommand = new DelegateCommand(new Action(this.ExecuteEraseFile), new Func<bool>(this.CanExecuteEraseFile));

            this.PictureInfoCommand = new DelegateCommand<object>(new Action<object>(this.ExecutePictureInfo), new Func<object, bool>(this.CanExecutePictureInfo));
        }

        #endregion


        #region  Commands Properties

        public DelegateCommand<object> OpenFileCommand { get; set; }

        public DelegateCommand ClosePictureCommand { get; set; }

        public DelegateCommand SaveAsCommand { get; set; }

        public DelegateCommand<object> HideFileCommand { get; set; }

        public DelegateCommand<object> RetrieveFileCommand { get; set; }

        public DelegateCommand EraseFileCommand { get; set; }

        public DelegateCommand<object> PictureInfoCommand { get; set; }


        #endregion


        #region Properites

        private BitmapImage coverImage = null;
        public BitmapImage CoverImage
        {
            get { return this.coverImage; }
            set
            {
                this.coverImage = value;
                RaisePropertyChanged("CoverImage");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        private BitmapImage hiddenImage = null;
        public BitmapImage HiddenImage
        {
            get { return this.hiddenImage; }
            set
            {
                this.hiddenImage = value;
                RaisePropertyChanged("HiddenImage");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }


        private Bitmap coverImageBitmapCache;
        
        private Bitmap hiddenImageBitmapCache;
       

        #endregion


        #region Commands

        public bool CanExecutePictureInfo(object sender)
        {
            return this.CoverImage != null; 
        }

        public void ExecutePictureInfo(object sender)
        {
            Application.Current.Properties["pictureInfo"] = PictureInfo.CreatePictureInfo(this.coverImageBitmapCache);
            (new PictureInfoViewModel()).doUpdate(sender as Window);
            Application.Current.Properties["pictureInfo"] = null;
        }


        public bool CanExecuteEraseFile()
        {
            return this.HiddenImage != null;
        }

        public void ExecuteEraseFile()
        {
            HideLSB.Erase(ref this.hiddenImageBitmapCache);
            this.HiddenImage = FileTransform.Bitmap2BitmapImage(this.hiddenImageBitmapCache);
            
            this.showMessageBox("擦除已完成.", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public bool CanExecuteSaveAs()
        {
            return  this.HiddenImage != null;
        }

        public void ExecuteSaveAs()
        {
            string fileTypesPattern = "bmp file (*.bmp)|*.bmp|All files (*.*)|*.*";
            string defaultExt = "bmp";
            string savePath = GetFilePathFromFileDialog.getFilePahtFromSaveFileDialog(fileTypesPattern, defaultExt);

            if (File.Exists(savePath))
                File.Delete(savePath);
            this.hiddenImageBitmapCache.Save(savePath);

            return;
        }



        public bool CanExecuteOpenFile(object args)
        {
            return true;
        }

        public void ExecuteOpenFile(object args)
        {
            try
            {
                string fileTypesPattern = "bmp file (*.bmp)|*.bmp|All files (*.*)|*.*";
                string defaultExt = "bmp";
                string imageUri = GetFilePathFromFileDialog.getFilePathFromOpenFileDialog(fileTypesPattern, defaultExt);

                if (string.IsNullOrEmpty(imageUri))
                    return;

                BitmapImage bitmapImg = FileTransform.ImageUri2BitmapImage(imageUri);
                Bitmap bitmapCache = FileTransform.BitmapImage2Bitmap(bitmapImg);

                string openCmdPara = (string)args;
                if ("Cover".Equals(openCmdPara))
                {
                    this.CoverImage = bitmapImg;
                    this.coverImageBitmapCache = bitmapCache;
                    this.HiddenImage = null;
                    this.hiddenImageBitmapCache = null;
                }
                else if ("Hidden".Equals(openCmdPara))
                {
                    this.HiddenImage = bitmapImg;
                    this.hiddenImageBitmapCache = bitmapCache;
                    this.CoverImage = null;
                    this.coverImageBitmapCache = null;
                }
            }
            catch (FileFormatException)
            {

                showMessageBox("请打开bmp文件.");
            }
        }



        public bool CanExecuteHideFile(object args)
        {
            return this.CoverImage != null;
        }

        public void ExecuteHideFile(object args)
        {            
            try
            {

                HideOption hideOption = this.getHideOptionFromView((Window)args);

                if (hideOption == null)
                    return;

                byte[] fileBytesToHide = FileTransform.File2ByteArray(hideOption.FilePath);
                byte[] zipedCoverImageBytes = ZIP.Compress(fileBytesToHide);
                byte[] ciphertext = EncryptionTEA.Encrypt(zipedCoverImageBytes, strPassword2uintArr(hideOption.Password));

                Bitmap tmpBitmapCacheToHide = (Bitmap)this.coverImageBitmapCache.Clone(new Rectangle(0, 0, this.coverImageBitmapCache.Width, this.coverImageBitmapCache.Height), this.coverImageBitmapCache.PixelFormat);

                HideLSB.Hide(ref tmpBitmapCacheToHide, ciphertext);
                byte[] outbytes = HideLSB.DeHide(tmpBitmapCacheToHide);

                this.hiddenImageBitmapCache = tmpBitmapCacheToHide;
                this.HiddenImage = FileTransform.Bitmap2BitmapImage(this.hiddenImageBitmapCache);


                Console.WriteLine("just for setting a breakpoint.");
            }
            catch (ImageHideCapacityTooSmallException)
            {
                showMessageBox("该图片隐藏容量不足", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        public bool CanExecuteRetrieveFile(object args)
        {
            return this.HiddenImage != null;
        }

        public void ExecuteRetrieveFile(object args)
        {
            try
            {
                DeHideOption deHideOption = this.getDeHideOptionFromView((Window)args);

                if (deHideOption == null)
                    return;

                byte[] ciphertext = HideLSB.DeHide(this.hiddenImageBitmapCache);
                byte[] plaintext = EncryptionTEA.Decrypt(ciphertext, strPassword2uintArr(deHideOption.Password));
                byte[] deZipdata = ZIP.Decompress(plaintext);

                if (File.Exists(deHideOption.FilePath))
                    File.Delete(deHideOption.FilePath);
                FileTransform.ByteArray2File(deHideOption.FilePath, deZipdata);

                showMessageBox("Retrieve file is done.", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (PasswordWrongException)
            {
                showMessageBox("Password is wrong.", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }


        public bool CanExecuteClosePicture()
        {
            return this.CoverImage != null ||  this.HiddenImage != null ;
        }

        public void ExecuteClosePicture()
        {
            bool isConfirm = this.showMessageBox("确定关闭？");

            if (isConfirm == false)
                return;

            this.CoverImage = null;
            this.coverImageBitmapCache = null;
            this.HiddenImage = null;
            this.hiddenImageBitmapCache = null;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="msgBoxCaption">标题栏标题</param>
        /// <param name="msgBoxBtn">按钮</param>
        /// <param name="msgBoxImg">图标</param>
        /// <returns></returns>
        private bool showMessageBox(string msg, 
            string msgBoxCaption = "提示", 
            MessageBoxButton msgBoxBtn = MessageBoxButton.YesNo,
            MessageBoxImage msgBoxImg = MessageBoxImage.Question)
        {
            
            MessageBoxResult isConfirm = MessageBox.Show(msg, msgBoxCaption, msgBoxBtn, msgBoxImg);
            if (isConfirm == MessageBoxResult.Yes || isConfirm == MessageBoxResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private HideOption getHideOptionFromView(Window parentWin)
        {
            
            var hovm = new HideOptionViewModel();
            hovm.doUpdate(parentWin);
            var hideOption = (HideOption) Application.Current.Properties["hideOption"];
            Application.Current.Properties["hideOption"] = null;
           
            return hideOption;
        }

        private DeHideOption getDeHideOptionFromView(Window parentWin)
        {
            var deHideOptionViewModel = new DeHideOptionViewModel();
            deHideOptionViewModel.doUpate(parentWin);
            var deHideOption = (DeHideOption) Application.Current.Properties["deHideOption"];
            Application.Current.Properties["deHideOption"] = null;
            
            return deHideOption;
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

        private uint[] strPassword2uintArr(string strPassword)
        {
            uint[] ret = new uint[4];

            string paddedStrPassword = string.Concat(strPassword, new string('0', 16-strPassword.Length));

            for (int i = 0; i < paddedStrPassword.Length; i += 4)
            {
                int idx = i / 4;
                ret[idx] = strPassword[idx + 3];
                ret[idx] |= (uint)((strPassword[idx + 2]) << 8);
                ret[idx] |= (uint)((strPassword[idx + 1]) << 16);
                ret[idx] |= (uint)((strPassword[idx    ]) << 24);
            }

            return ret;
        }


    }
}

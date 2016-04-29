using InfoHidden.Model;
using InfoHidden.Utility;
using InfoHidden.View;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace InfoHidden.ViewModel
{
    public class DeHideOptionViewModel : NotificationObject, IDataErrorInfo
    {

        #region ctor

        public DeHideOptionViewModel()
        {
            this.SubmitCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteSubmit), new Func<object, bool>(this.CanExecuteSubmit));
            this.CancelCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteCancel), new Func<object, bool>(this.CanExecuteCancel));

            this.OpenFileCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteOpenFile), new Func<object, bool>(this.CanExecuteOpenFile));
        }

        #endregion


        #region Properties

        public DelegateCommand<object> SubmitCommand { get; set; }

        public DelegateCommand<object> CancelCommand { get; set; }

        public DelegateCommand<object> OpenFileCommand { get; set; }

        private string _password;
        public string Password
        {
            get { return this._password; }
            set
            {
                this._password = value;
                RaisePropertyChanged("Password");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return this._filePath; }
            set
            {
                this._filePath = value;
                RaisePropertyChanged("FilePath");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        private string _encryptionAlg;
        public string EncryptionAlg
        {
            get { return this._encryptionAlg; }
            set
            {
                this._encryptionAlg = value;
                RaisePropertyChanged("ENCRYPTIONALG");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public List<String> EncryptionAlgs
        {
            get { return new List<string> { "TEA", "AES", "DES", "Rijndael" }; }
        }

        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName.Equals("Password"))
                    return this.ValidatePassword(this.Password);

                throw new InvalidOperationException();
            }
        }

        #endregion

        #region

        private string ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "";

            string pattern = @"^\w{8,16}$";

            if (!Regex.IsMatch(password, pattern))
                return "请输入8-16位字符.";

            return "";
        }

        #endregion

        #region Commands

        public bool CanExecuteSubmit(object args)
        {
            return !string.Empty.Equals(this.Password) && !string.Empty.Equals(this.FilePath);
        }

        public void ExecuteSubmit(object args)
        {
            DeHideOption deHideOption = new DeHideOption { Password = this.PaddingPassword(), FilePath = this.FilePath, EncryptionAlg = this.EncryptionAlg};
            Application.Current.Properties["deHideOption"] = deHideOption;
            
            ((Window)args).Close();
        }



        public bool CanExecuteCancel(object args)
        {
            return true;
        }

        public void ExecuteCancel(object args)
        {
            ( (Window)args ).Close();
        }



        public bool CanExecuteOpenFile(object args)
        {
            return true;
        }

        public void ExecuteOpenFile(object args)
        {
            string fileTypesPattern = "All files (*.*)|*.*";
            string defaultExt = string.Empty;
            this.FilePath = GetFilePathFromFileDialog.getFilePahtFromSaveFileDialog(fileTypesPattern, defaultExt);
        }


        #endregion

        public void DoUpate(Window parentWin)
        {
            Window win = new DeHideOptionView();
            win.Owner = parentWin;

            win.ShowDialog();

        }

        private void RaiseAllCommandsCanExecuteChanged()
        {
            this.SubmitCommand.RaiseCanExecuteChanged();
            this.CancelCommand.RaiseCanExecuteChanged();
            this.OpenFileCommand.RaiseCanExecuteChanged();
        }

        private string PaddingPassword()
        {
            int shouldLen = 0;

            if ("DES".Equals(this.EncryptionAlg))
                shouldLen = 16;
            else if ("TEA".Equals(this.EncryptionAlg))
                shouldLen = 16;
            else if ("AES".Equals(this.EncryptionAlg))
                shouldLen = 32;
            else if ("Rijndael".Equals(this.EncryptionAlg))
                shouldLen = 32;

            return string.Concat(this.Password, new string('0', shouldLen - this.Password.Length));
        }
    }
}

using InfoHidden.Model;
using InfoHidden.Utility;
using InfoHidden.View;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace InfoHidden.ViewModel
{
    class HideOptionViewModel : NotificationObject, IDataErrorInfo
    {

        public HideOptionViewModel()
        {

            this.OpenFileCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteOpenFile), new Func<object, bool>(this.CanExecuteOpenFile));

            this.SubmitCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteSubmit), new Func<object, bool>(this.CanExecuteSubmit));
            this.CancelCommand = new DelegateCommand<object>(new Action<object>(this.ExecuteCancel), new Func<object, bool>(this.CanExecuteCancel));
        }

        #region Properties

        private string _password = string.Empty;

        private string _confirmPassword = string.Empty;

        private string _encryptionAlg = string.Empty;

        private string _filePath = string.Empty;

        public string Password
        {
            get
            {
                return this._password;
            }

            set
            {
                this._password = value;
                RaisePropertyChanged("PASSWORD");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get { return this._confirmPassword; }
            set
            {
                this._confirmPassword = value;
                RaisePropertyChanged("CONFIRMPASSWORD");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

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

        public string FilePath
        {
            get { return this._filePath; }
            set
            {
                this._filePath = value;
                RaisePropertyChanged("FILEPATH");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public DelegateCommand<object> OpenFileCommand { get; set; }

        public DelegateCommand<object> SubmitCommand { get; set; }

        public DelegateCommand<object> CancelCommand { get; set; }

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
                   
                if (columnName.Equals("ConfirmPassword"))
                    return this.ValidateConfirmPassword(this.ConfirmPassword, this.Password);

                throw new InvalidOperationException();
            }
        }

        #endregion

        #region Validation

        private string ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "";

            string pattern = @"^\w{8,16}$";

            if ( ! Regex.IsMatch(password, pattern))
                return "请输入8-16位字符.";

            return "";
        }

        private string ValidateConfirmPassword(string confirmPassword, string password)
        {
            if (string.IsNullOrEmpty(password))
                return "";

            if (!confirmPassword.Equals(password))
                return "两次输入的密码不一致.";

            return "";
        }

        #endregion

        #region Commands

        public bool CanExecuteOpenFile(object args)
        {
            return true;
        }

        public void ExecuteOpenFile(object args)
        {
            string fileTypesPattern = "All files (*.*)|*.*";
            string defaultExt = string.Empty;
            this.FilePath = GetFilePathFromFileDialog.getFilePathFromOpenFileDialog(fileTypesPattern, defaultExt);
        }

        public bool CanExecuteSubmit(object args)
        {
            if (   String.IsNullOrEmpty(this.Password)
                || String.IsNullOrEmpty(this.ConfirmPassword)
                || String.IsNullOrEmpty(this.EncryptionAlg)
                || String.IsNullOrEmpty(this.FilePath)
                )
            {
                return false;
            }

            return true;
        }

        public void ExecuteSubmit(object args)
        {


            HideOption hideOption = new HideOption() { Password = this.PaddingPassword(), EncryptionAlg = this.EncryptionAlg, FilePath = this.FilePath };

            Application.Current.Properties["hideOption"] = hideOption;

            (args as Window).Close();
        }


        public bool CanExecuteCancel(object args)
        {
            return true;
        }

        public void ExecuteCancel(object args)
        {
            (args as Window).Close();
        }



        #endregion

        public void DoUpdate(Window parentWin)
        {
            Window win = new HideOptionView();
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

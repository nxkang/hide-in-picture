namespace InfoHidden.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows;

    using InfoHidden.Model;
    using InfoHidden.Utility;
    using InfoHidden.View;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;

    public class HideOptionViewModel : NotificationObject, IDataErrorInfo
    {
        #region Fields

        private string _confirmPassword = string.Empty;

        private string _encryptionAlg = string.Empty;

        private string _filePath = string.Empty;

        private string _password = string.Empty;

        #endregion

        #region Constructors and destructors

        public HideOptionViewModel()
        {
            this.OpenFileCommand = new DelegateCommand<object>(
                new Action<object>(this.ExecuteOpenFile), 
                new Func<object, bool>(this.CanExecuteOpenFile));

            this.SubmitCommand = new DelegateCommand<object>(
                new Action<object>(this.ExecuteSubmit), 
                new Func<object, bool>(this.CanExecuteSubmit));
            this.CancelCommand = new DelegateCommand<object>(
                new Action<object>(this.ExecuteCancel), 
                new Func<object, bool>(this.CanExecuteCancel));
        }

        #endregion

        #region Public properties

        public DelegateCommand<object> CancelCommand { get; set; }

        public string ConfirmPassword
        {
            get
            {
                return this._confirmPassword;
            }

            set
            {
                this._confirmPassword = value;
                this.RaisePropertyChanged("CONFIRMPASSWORD");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public string EncryptionAlg
        {
            get
            {
                return this._encryptionAlg;
            }

            set
            {
                this._encryptionAlg = value;
                this.RaisePropertyChanged("ENCRYPTIONALG");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public List<string> EncryptionAlgs
        {
            get
            {
                return new List<string> { "TEA", "AES", "DES", "Rijndael" };
            }
        }

        public string Error
        {
            get
            {
                return null;
            }
        }

        public string FilePath
        {
            get
            {
                return this._filePath;
            }

            set
            {
                this._filePath = value;
                this.RaisePropertyChanged("FILEPATH");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public DelegateCommand<object> OpenFileCommand { get; set; }

        public string Password
        {
            get
            {
                return this._password;
            }

            set
            {
                this._password = value;
                this.RaisePropertyChanged("PASSWORD");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public DelegateCommand<object> SubmitCommand { get; set; }

        #endregion

        #region Public indexers

        public string this[string columnName]
        {
            get
            {
                if (columnName.Equals("Password"))
                {
                    return this.ValidatePassword(this.Password);
                }

                if (columnName.Equals("ConfirmPassword"))
                {
                    return this.ValidateConfirmPassword(this.ConfirmPassword, this.Password);
                }

                throw new InvalidOperationException();
            }
        }

        #endregion

        #region Public methods

        public bool CanExecuteCancel(object args)
        {
            return true;
        }

        public bool CanExecuteOpenFile(object args)
        {
            return true;
        }

        public bool CanExecuteSubmit(object args)
        {
            if (string.IsNullOrEmpty(this.Password) || string.IsNullOrEmpty(this.ConfirmPassword)
                || string.IsNullOrEmpty(this.EncryptionAlg) || string.IsNullOrEmpty(this.FilePath))
            {
                return false;
            }

            return true;
        }

        public void DoUpdate(Window parentWin)
        {
            Window win = new HideOptionView();
            win.Owner = parentWin;

            win.ShowDialog();
        }

        public void ExecuteCancel(object args)
        {
            (args as Window).Close();
        }

        public void ExecuteOpenFile(object args)
        {
            string fileTypesPattern = "All files (*.*)|*.*";
            string defaultExt = string.Empty;
            this.FilePath = GetFilePathFromFileDialog.GetFilePathFromOpenFileDialog(fileTypesPattern, defaultExt);
        }

        public void ExecuteSubmit(object args)
        {
            HideOption hideOption = new HideOption()
                                        {
                                            Password = this.PaddingPassword(), 
                                            EncryptionAlg = this.EncryptionAlg, 
                                            FilePath = this.FilePath
                                        };

            Application.Current.Properties["hideOption"] = hideOption;

            var window = args as Window;
            if (window != null) window.Close();
        }

        #endregion

        #region Other methods

        private string PaddingPassword()
        {
            int shouldLen = 0;

            if ("DES".Equals(this.EncryptionAlg)) shouldLen = 16;
            else if ("TEA".Equals(this.EncryptionAlg)) shouldLen = 16;
            else if ("AES".Equals(this.EncryptionAlg)) shouldLen = 32;
            else if ("Rijndael".Equals(this.EncryptionAlg)) shouldLen = 32;

            return string.Concat(this.Password, new string('0', shouldLen - this.Password.Length));
        }

        private void RaiseAllCommandsCanExecuteChanged()
        {
            this.SubmitCommand.RaiseCanExecuteChanged();
            this.CancelCommand.RaiseCanExecuteChanged();
            this.OpenFileCommand.RaiseCanExecuteChanged();
        }

        private string ValidateConfirmPassword(string confirmPassword, string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            if (!confirmPassword.Equals(password)) return "两次输入的密码不一致.";

            return string.Empty;
        }

        private string ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            string pattern = @"^\w{8,16}$";

            if (!Regex.IsMatch(password, pattern)) return "请输入8-16位字符.";

            return string.Empty;
        }

        #endregion
    }
}
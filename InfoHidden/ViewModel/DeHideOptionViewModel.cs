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

    public class DeHideOptionViewModel : NotificationObject, IDataErrorInfo
    {
        #region Fields

        private string _encryptionAlg;

        private string _filePath;

        private bool _isValid;

        private string _password;

        #endregion

        #region Constructors and destructors

        public DeHideOptionViewModel()
        {
            this.SubmitCommand = new DelegateCommand<object>(
                new Action<object>(this.ExecuteSubmit), 
                new Func<object, bool>(this.CanExecuteSubmit));
            this.CancelCommand = new DelegateCommand<object>(
                new Action<object>(this.ExecuteCancel), 
                new Func<object, bool>(this.CanExecuteCancel));

            this.OpenFileCommand = new DelegateCommand<object>(
                new Action<object>(this.ExecuteOpenFile), 
                new Func<object, bool>(this.CanExecuteOpenFile));
        }

        #endregion

        #region Public properties

        public DelegateCommand<object> CancelCommand { get; set; }

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
                this.RaisePropertyChanged("FilePath");

                this.RaiseAllCommandsCanExecuteChanged();
            }
        }

        public bool IsValid
        {
            get
            {
                this.ValidatePassword(this.Password);
                return this._isValid;
            }

            set
            {
                this._isValid = value;
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
                this.RaisePropertyChanged("Password");

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
                if (columnName.Equals("Password")) return this.ValidatePassword(this.Password);

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
            return !string.IsNullOrEmpty(this.Password) && !string.IsNullOrEmpty(this.FilePath)
                   && !string.IsNullOrEmpty(this.EncryptionAlg) && this.IsValid;
        }

        public void DoUpate(Window parentWin)
        {
            Window win = new DeHideOptionView();
            win.Owner = parentWin;

            win.ShowDialog();
        }

        public void ExecuteCancel(object args)
        {
            ((Window)args).Close();
        }

        public void ExecuteOpenFile(object args)
        {
            string fileTypesPattern = "All files (*.*)|*.*";
            string defaultExt = string.Empty;
            this.FilePath = GetFilePathFromFileDialog.GetFilePahtFromSaveFileDialog(fileTypesPattern, defaultExt);
        }

        public void ExecuteSubmit(object args)
        {
            DeHideOption deHideOption = new DeHideOption
                                            {
                                                Password = this.PaddingPassword(), 
                                                FilePath = this.FilePath, 
                                                EncryptionAlg = this.EncryptionAlg
                                            };
            Application.Current.Properties["deHideOption"] = deHideOption;

            ((Window)args).Close();
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

        private string ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                this._isValid = false;
                return string.Empty;
            }

            string pattern = @"^\w{8,16}$";

            if (!Regex.IsMatch(password, pattern))
            {
                this._isValid = false;
                return "请输入8-16位字符.";
            }

            this._isValid = true;
            return string.Empty;
        }

        #endregion
    }
}
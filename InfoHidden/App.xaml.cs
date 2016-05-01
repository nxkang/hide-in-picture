using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace InfoHidden
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string Culture { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GetCulture();
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SetCulture();

            MessageBoxManager.Unregister();
        }

        #region Method
        private void GetCulture()
        {
            Culture = string.Empty;
            try
            {
                Culture = InfoHidden.Properties.Settings.Default.Culture.Trim();
            }
            catch (Exception)
            {
            }
            Culture = string.IsNullOrEmpty(Culture) ? "zh-CN" : Culture;

            //update culture
            UpdateCulture();
        }
        private void SetCulture()
        {
            try
            {
                InfoHidden.Properties.Settings.Default.Culture = Culture;
                InfoHidden.Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
            }
        }
        public static void UpdateCulture()
        {

            if (String.IsNullOrEmpty(Culture))
                return;

            //Copy all MergedDictionarys into a auxiliar list.
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

            //Search for the specified culture.     
            string requestedCulture = string.Format(@"ResourceDictionary/Lang/{0}.xaml", Culture);
            var resourceDictionary = dictionaryList.
                FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

            if (resourceDictionary == null)
            {
                //If not found, select our default language.             
                requestedCulture = @"ResourceDictionary/Lang/en-US.xaml";
                resourceDictionary = dictionaryList.
                    FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            }

            //If we have the requested resource, remove it from the list and place at the end.     
            //Then this language will be our string table to use.      
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            //Inform the threads of the new culture.     
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Culture);

            MessageBoxManager.Unregister();
            //MessageBoxManager.OK = Application.Current.TryFindResource("BtnOK").ToString();
            MessageBoxManager.Register();
        
        }
        #endregion
    }
}

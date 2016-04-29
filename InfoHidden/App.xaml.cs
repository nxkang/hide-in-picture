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
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format(@"ResourceDictionary/Lang/{0}.xaml", Culture);

            ResourceDictionary resourceDictionary = dictionaryList[0];

            if (resourceDictionary == null)
            {
                requestedCulture = @"ResourceDictionary/Lang/en-US.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            }
            if (resourceDictionary != null)
            {
                var oldDicts = Application.Current.Resources.MergedDictionaries;
                
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InfoHidden.Utility
{
    using System.Windows;

    public class GetFilePathFromFileDialog
    {
        public static string GetFilePathFromOpenFileDialog(string fileTypesPattern, string defaultExt)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = Application.Current.FindResource("ChooseFile") as string;
            openFileDialog.Filter = fileTypesPattern;
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = defaultExt;

            var result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return string.Empty;
            }
            
            return openFileDialog.FileName;
        }

        public static string GetFilePahtFromSaveFileDialog(string fileTypesPattern, string defaultExt)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

            saveFileDialog.Filter = fileTypesPattern;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.DefaultExt = defaultExt;
            saveFileDialog.RestoreDirectory = true;

            var result = saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return String.Empty;
            }
            
            return saveFileDialog.FileName;
        }
		
    }
}

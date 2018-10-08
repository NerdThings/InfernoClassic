#if WINDOWS_UWP

using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace Inferno.UI
{
    /// <summary>
    /// UWP Specific MessageBox code
    /// </summary>
    internal class PlatformMessageBox
    {
        public static async void Show(string title, string message, MessageBoxType type)
        {
            var messageBox = new ContentDialog
            {
                Title = title,
                Content = type,
                CloseButtonText = "Ok"
            };

            await messageBox.ShowAsync();
        }
    }
}

#endif
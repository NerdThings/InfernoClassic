using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.UI
{
    public static class MessageBox
    {
        public static void Show(string title, string message)
        {
            PlatformMessageBox.Show(title, message);
        }
    }
}

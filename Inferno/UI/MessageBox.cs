﻿using System.Threading.Tasks;

namespace Inferno.UI
{
    /// <summary>
    /// The type of message box to be displayed
    /// </summary>
    public enum MessageBoxType
    {
        /// <summary>
        /// An information box
        /// </summary>
        Information,

        /// <summary>
        /// A warning box
        /// </summary>
        Warning,

        /// <summary>
        /// An error box
        /// </summary>
        Error
    }

    /// <summary>
    /// MessageBox Provider
    /// </summary>
    public static partial class MessageBox
    {
        /// <summary>
        /// Display a native message box
        /// </summary>
        /// <param name="title">Title of the box</param>
        /// <param name="message">Box message</param>
        /// <param name="type">Box type</param>
        /// <param name="async">Run the box in the background (Allow code to continue running)</param>
        public static void Show(string title, string message, MessageBoxType type = MessageBoxType.Information, bool async = false)
        {
            if (!async)
                Display(title, message, type);
            else
                Task.Run(() => Show(title, message, type));
        }
    }
}

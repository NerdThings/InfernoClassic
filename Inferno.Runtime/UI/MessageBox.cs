namespace Inferno.Runtime.UI
{
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

    public static class MessageBox
    {
        public static void Show(string title, string message, MessageBoxType type = MessageBoxType.Information)
        {
            PlatformMessageBox.Show(title, message, type);
        }
    }
}

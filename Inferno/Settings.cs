namespace Inferno
{
    /// <summary>
    /// Inferno settings
    /// </summary>
    public static class Settings
    {
        #region Exceptions

        /// <summary>
        /// Disable the Attempt to per pixel check an animated sprite.
        /// Can lead to unexpected collision issues.
        /// </summary>
        public static bool AttemptToPerPixelCheckAnimation = true;

        #endregion
    }
}
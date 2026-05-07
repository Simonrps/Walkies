namespace Walkies.MAUI.Utilities
{
    /// <summary>
    /// Contains constants used across the MAUI application
    /// </summary>
    public static class ApiConstants
    {
        /// <summary>
        /// the base url of the API. 
        /// </summary>
        public static Uri BaseUrl =>
#if ANDROID
    new("http://10.0.2.2:5007/api/");
#else
    new("http://localhost:5007/api/");
#endif
    }
}
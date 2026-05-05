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
    new("https://10.0.2.2:7001/api");
#else
    new("https://localhost:7001/api");
#endif
    }
}

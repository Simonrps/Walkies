namespace Walkies.MAUI.Services
{
    /// <summary>
    /// Wraps MAUI SecureStorage for use via dependency injection.
    /// Falls back to preferences on windows where SecureStorage
    /// has limitations with the PasswordVault API
    /// </summary>
    public class SecureStorageService : ISecureStorageService
    {
        /// <summary>
        /// Retrieves a stored value by key. Falls back to preferences
        /// on windows if securestorage fails
        /// </summary>
        public async Task<string?> GetAsync(string key)
        {
            try
            {
                return await SecureStorage.Default.GetAsync(key);
            }
            catch (ArgumentException)
            {
                return Preferences.Default.Get<string?>(key, null);
            }
        }

        /// <summary>
        /// Stores a value by key. Falls back to preferences on 
        /// windows if securestorage fails
        /// </summary>
        public async Task SetAsync(string key, string value)
        {
            try
            {
                await SecureStorage.Default.SetAsync(key, value);
            }
            catch (ArgumentException)
            {
                Preferences.Default.Set(key, value);
            }
        }

        /// <summary>
        /// Removes a stored value by key from both secureStorage and preferences
        /// </summary>
        public void Remove(string key)
        {
            SecureStorage.Default.Remove(key);
            Preferences.Default.Remove(key);
        }
    }
}
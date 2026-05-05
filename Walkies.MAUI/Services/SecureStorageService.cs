namespace Walkies.MAUI.Services
{
    /// <summary>
    /// Wraps MAUI SecureStorage for use via dependency injection
    /// </summary>
    public class SecureStorageService : ISecureStorageService
    {
        public async Task<string?> GetAsync(string key) =>
            await SecureStorage.Default.GetAsync(key);

        public async Task SetAsync(string key, string value) =>
            await SecureStorage.Default.SetAsync(key, value);

        public void Remove(string key) =>
            SecureStorage.Default.Remove(key);
    }
}
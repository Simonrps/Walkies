namespace Walkies.MAUI.Services
{
    /// <summary>
    /// Absrtaction over SecureStorage to allow instance-based
    /// access and support for dependency injection
    /// </summary>
    public interface ISecureStorageService
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value);
        void Remove(string key);
    }
}
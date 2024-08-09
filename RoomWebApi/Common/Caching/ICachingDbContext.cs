namespace RoomWebApi.Common.Caching
{
    public interface ICachingDbContext
    {
        T? GetData<T>(string key) where T : class;
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveDate(string key);
    }
}

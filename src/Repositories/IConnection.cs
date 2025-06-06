namespace Sefer.Backend.Donations.Api.Repositories;

public interface IConnection : IDisposable
{
    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);

    public Task<int> ExecuteAsync(string sql, object? param = null);
}
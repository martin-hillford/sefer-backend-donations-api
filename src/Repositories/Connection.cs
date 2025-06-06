using System.Diagnostics.CodeAnalysis;

namespace Sefer.Backend.Donations.Api.Repositories;

[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
public class Connection(NpgsqlConnection connection) : IConnection, IAsyncDisposable
{
    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null) => connection.QueryAsync<T>(sql, param);
    
    public Task<int> ExecuteAsync(string sql, object? param = null) => connection.ExecuteAsync(sql, param);

    public void Dispose()  => connection.Dispose();

    public ValueTask DisposeAsync() => connection.DisposeAsync();
}
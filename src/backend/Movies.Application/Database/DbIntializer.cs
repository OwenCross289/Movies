using Dapper;
using Npgsql;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly NpgsqlConnection _connection;

    public DbInitializer(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task InitializeAsync()
    { 
        await _connection.OpenAsync();

        await _connection.ExecuteAsync(Sql.CreateMoviesTable);
        await _connection.ExecuteAsync(Sql.CreateSlugIndex);
        await _connection.ExecuteAsync(Sql.CreateGenresTable);
        await _connection.ExecuteAsync(Sql.CreateRatingsTable);
    }
}
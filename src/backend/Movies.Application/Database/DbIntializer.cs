using Dapper;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync(Sql.CreateMoviesTable);
        await connection.ExecuteAsync(Sql.CreateSlugIndex);
        await connection.ExecuteAsync(Sql.CreateGenresTable);
        await connection.ExecuteAsync(Sql.CreateRatingsTable);
    }
}
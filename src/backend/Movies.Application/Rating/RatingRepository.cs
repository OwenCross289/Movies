using Dapper;
using Movies.Application.Database;
using Npgsql;

namespace Movies.Application.Rating;

public class RatingRepository : IRatingRepository
{
    private readonly NpgsqlConnection _connection;
    public RatingRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var result = await _connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition(Sql.GetRatingByMovieId,
            new { movieId }, cancellationToken: token));
        await _connection.CloseAsync();
        return result;
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId,
        CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var result = await _connection.QuerySingleOrDefaultAsync<(float? Rating, int? UserRating)>(new CommandDefinition(
            Sql.GetRatingByMovieIdAndUserid,
            new { movieId, userId }, cancellationToken: token));
        await _connection.CloseAsync();
        return result;
    }
}
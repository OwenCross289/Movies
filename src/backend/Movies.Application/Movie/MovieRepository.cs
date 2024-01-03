using Dapper;
using Movies.Application.Database;
using Npgsql;

namespace Movies.Application.Movie;

public class MovieRepository : IMovieRepository
{
    private readonly NpgsqlConnection _connection;

    public MovieRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var transaction = await _connection.BeginTransactionAsync(token);

        var result = await _connection.ExecuteAsync(new CommandDefinition(Sql.CreateMovieWithoutGenre, movie));
        if (result > 0)
            foreach (var genre in movie.Genres)
                await _connection.ExecuteAsync(new CommandDefinition(Sql.InsertGenresIntoMovie,
                    new { MovieId = movie.Id, name = genre }, cancellationToken: token));
        await transaction.CommitAsync(token);
        await _connection.CloseAsync();
        
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var movie = await _connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(Sql.GetMovieById,
            new { id, userId }, cancellationToken: token));
        await _connection.CloseAsync();
        if (movie is null) return null;

        await _connection.OpenAsync(token);
        var genres =
            await _connection.QueryAsync<string>(new CommandDefinition(Sql.GetGenresForMovieById, new { id },
                cancellationToken: token));
        foreach (var genre in genres) movie.Genres.Add(genre);
        await _connection.CloseAsync();
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var movie = await _connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(Sql.GetMovieBySlug,
            new { slug, userId }, cancellationToken: token));
        await _connection.CloseAsync();
        
        if (movie is null) return null;

        await _connection.OpenAsync(token);
        var genres = await _connection.QueryAsync<string>(new CommandDefinition(Sql.GetGenresForMovieById,
            new { id = movie.Id }, cancellationToken: token));
        foreach (var genre in genres) movie.Genres.Add(genre);
        await _connection.CloseAsync();
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var movies =
            await _connection.QueryAsync(new CommandDefinition(Sql.GetAllMovies, new { userId },
                cancellationToken: token));
        await _connection.CloseAsync();
        
        return movies.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Rating = (float?)x.rating,
            UserRating = (int?)x.userrating,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var transaction = await _connection.BeginTransactionAsync(token);

        await _connection.ExecuteAsync(new CommandDefinition(Sql.DeleteGenresByMovieId, new { id = movie.Id },
            cancellationToken: token));

        foreach (var genre in movie.Genres)
            await _connection.ExecuteAsync(new CommandDefinition(Sql.InsertGenresIntoMovie,
                new { MovieId = movie.Id, name = genre }));

        var result =
            await _connection.ExecuteAsync(new CommandDefinition(Sql.UpdateMovieById, movie, cancellationToken: token));
        await transaction.CommitAsync(token);
        await _connection.CloseAsync();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var transaction = await _connection.BeginTransactionAsync(token);

        await _connection.ExecuteAsync(new CommandDefinition(Sql.DeleteGenresByMovieId, new { id },
            cancellationToken: token));
        var result =
            await _connection.ExecuteAsync(new CommandDefinition(Sql.DeleteMovieById, new { id },
                cancellationToken: token));
        await transaction.CommitAsync(token);
        await _connection.CloseAsync();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        var result = await _connection.ExecuteScalarAsync<bool>(new CommandDefinition(Sql.MovieExistsById, new { id }));
        await _connection.CloseAsync();
        return result;
    }
}
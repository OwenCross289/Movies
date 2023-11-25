using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Movie;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(Sql.CreateMovieWithoutGenre, movie));
        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition(Sql.InsertGenresIntoMovie, new {MovieId = movie.Id, name = genre}, cancellationToken: token));
            }
        }
        transaction.Commit();
        
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(Sql.GetMovieById, new { id, userId }, cancellationToken: token));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition(Sql.GetGenresForMovieById, new { id }, cancellationToken: token));
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(Sql.GetMovieBySlug, new { slug, userId }, cancellationToken: token));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition(Sql.GetGenresForMovieById, new { id = movie.Id }, cancellationToken: token));
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }
    
    public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movies = await connection.QueryAsync(new CommandDefinition(Sql.GetAllMovies, new { userId }, cancellationToken: token));

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
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition(Sql.DeleteGenresByMovieId, new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition(Sql.InsertGenresIntoMovie,
                new { MovieId = movie.Id, name = genre }));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition(Sql.UpdateMovieById, movie, cancellationToken: token));
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition(Sql.DeleteGenresByMovieId, new {id}, cancellationToken: token));
        var result = await connection.ExecuteAsync(new CommandDefinition(Sql.DeleteMovieById, new {id}, cancellationToken: token));
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(Sql.MovieExistsById, new { id }));
    }
}
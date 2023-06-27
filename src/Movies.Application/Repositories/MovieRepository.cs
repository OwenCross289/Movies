using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(Sql.CreateMovieWithoutGenre, movie));
        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition(Sql.InsertGenresIntoMovie, new {MovieId = movie.Id, name = genre}));
            }
        }
        transaction.Commit();
        
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(Sql.GetMovieById, new { id }));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition(Sql.GetGenresForMovieById, new { id }));
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(Sql.GetMovieBySlug, new { slug }));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition(Sql.GetGenresForMovieById, new { id = movie.Id }));
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }
    
    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movies = await connection.QueryAsync(new CommandDefinition(Sql.GetAllMovies));

        return movies.Select(x => new Movie
        {
            Id = x.id, 
            Title = x.title, 
            YearOfRelease = x.yearofrelease, 
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition(Sql.DeleteGenresByMovieId, new { id = movie.Id }));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition(Sql.InsertGenresIntoMovie,
                new { MovieId = movie.Id, name = genre }));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition(Sql.UpdateMovieById, movie));
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition(Sql.DeleteGenresByMovieId, new {id}));
        var result = await connection.ExecuteAsync(new CommandDefinition(Sql.DeleteMovieById, new {id}));
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(Sql.MovieExistsById, new { id }));
    }
}
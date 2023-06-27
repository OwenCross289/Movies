using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie ToMovie(this CreateMovieRequest request)
    {
        return new Movie()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse ToResponse(this Movie movie)
    {
        return new MovieResponse(movie.Id, movie.Title, movie.Slug, movie.YearOfRelease, movie.Genres);
    }

    public static MoviesResponse ToResponse(this IEnumerable<Movie> movies)
    {
       return new MoviesResponse(movies.Select(ToResponse));
    }
    
    public static Movie ToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie()
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }
}
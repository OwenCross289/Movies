using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator)
    {
        _movieRepository = movieRepository;
        _movieValidator = movieValidator;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        return await _movieRepository.CreateAsync(movie, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token)
    {
        return _movieRepository.GetByIdAsync(id, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token)
    {
        return _movieRepository.GetBySlugAsync(slug, token);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token)
    {
        return _movieRepository.GetAllAsync(token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, token);
        if (!movieExists)
        {
            return null;
        }

        await _movieRepository.UpdateAsync(movie, token);
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token)
    {
        return _movieRepository.DeleteByIdAsync(id, token);
    }
}
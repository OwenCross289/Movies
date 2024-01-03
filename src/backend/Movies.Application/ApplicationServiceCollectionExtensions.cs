using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Movie;
using Movies.Application.Rating;

namespace Movies.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IRatingRepository, RatingRepository>();
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<DbInitializer>();
        return services;
    }
}
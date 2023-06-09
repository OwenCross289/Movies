using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.ToMovie();
        await _movieService.CreateAsync(movie, token);
        return CreatedAtAction(
            nameof(Get), 
            new {idOrSlug = movie.Id},
            movie.ToResponse());
    }
    
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var movie = Guid.TryParse(idOrSlug, out var id) ? 
            await _movieService.GetByIdAsync(id) : await _movieService.GetBySlugAsync(idOrSlug, token);
         if (movie is null)
         {
            return NotFound();
         }
         
         return Ok(movie.ToResponse());
    }
    
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var movies = await _movieService.GetAllAsync(token);
        return Ok(movies.ToResponse());
    }
    
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id,[FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var movie = request.ToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, token);
        if (updatedMovie is null)
        {
            return NotFound();
        }
        return Ok(updatedMovie.ToResponse()); 
    }
    
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _movieService.DeleteByIdAsync(id, token);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();  
    }
}
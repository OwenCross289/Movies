using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Movie;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;
/*
 Admin: True. TrustedUser: True
 eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkMjM5NjkwZS05YTRlLTRhODQtYmRkMC1jNTgwOTdhN2Y3ZGEiLCJzdWIiOiJPd2VuQ3Jvc3M5OUBHbWFpbC5jb20iLCJlbWFpbCI6Ik93ZW5Dcm9zczk5QEdtYWlsLmNvbSIsInVzZXJpZCI6ImQ4NTY2ZGUzLWIxYTYtNGE5Yi1iODQyLThlMzg4N2E4MmU0MSIsImFkbWluIjp0cnVlLCJ0cnVzdGVkX21lbWJlciI6dHJ1ZSwibmJmIjoxNjkwODI1OTc2LCJleHAiOjE2OTA4NTQ3NzYsImlhdCI6MTY5MDgyNTk3NiwiaXNzIjoiT3dlbkNyb3NzIiwiYXVkIjoiTW92aWVzLk93ZW5Dcm9zcyJ9.QAMRAQ8F-YcuivrQhLXtezBJRZYCT1KGVRJu2JKsWek

 Admin: False. TrustedUser: True
 eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0MWViM2RkYi1jZTA4LTQxODItOTE2MS1mZmVhMmRmNDEyMGEiLCJzdWIiOiJPd2VuQ3Jvc3M5OUBHbWFpbC5jb20iLCJlbWFpbCI6Ik93ZW5Dcm9zczk5QEdtYWlsLmNvbSIsInVzZXJpZCI6ImQ4NTY2ZGUzLWIxYTYtNGE5Yi1iODQyLThlMzg4N2E4MmU0MSIsImFkbWluIjpmYWxzZSwidHJ1c3RlZF9tZW1iZXIiOnRydWUsIm5iZiI6MTY5MDgyNjA2OSwiZXhwIjoxNjkwODU0ODY5LCJpYXQiOjE2OTA4MjYwNjksImlzcyI6Ik93ZW5Dcm9zcyIsImF1ZCI6Ik1vdmllcy5Pd2VuQ3Jvc3MifQ.8bKgGsFS9A_-euWiScdMQDtKPBWCHB8YOZNEdraHHXY

 Admin: False. TrustedUser: False
 eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIxNjEzZjZlYy0xMGFhLTQzYmQtOGU0Zi05YTRiZWZhMjcyZDciLCJzdWIiOiJPd2VuQ3Jvc3M5OUBHbWFpbC5jb20iLCJlbWFpbCI6Ik93ZW5Dcm9zczk5QEdtYWlsLmNvbSIsInVzZXJpZCI6ImQ4NTY2ZGUzLWIxYTYtNGE5Yi1iODQyLThlMzg4N2E4MmU0MSIsImFkbWluIjpmYWxzZSwidHJ1c3RlZF9tZW1iZXIiOmZhbHNlLCJuYmYiOjE2OTA4MjYxMTQsImV4cCI6MTY5MDg1NDkxNCwiaWF0IjoxNjkwODI2MTE0LCJpc3MiOiJPd2VuQ3Jvc3MiLCJhdWQiOiJNb3ZpZXMuT3dlbkNyb3NzIn0.rVmz8BD9xFSt8Y8f0CPLQ4SrVT2qu4hU1GDlqVo8NXQ
 */

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
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
    
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        
        var movie = Guid.TryParse(idOrSlug, out var id) ? 
            await _movieService.GetByIdAsync(id, userId, token) : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
         if (movie is null)
         {
            return NotFound();
         }
         
         return Ok(movie.ToResponse());
    }
    
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movies = await _movieService.GetAllAsync(userId, token);
        return Ok(movies.ToResponse());
    }
    
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id,[FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.ToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
        if (updatedMovie is null)
        {
            return NotFound();
        }
        return Ok(updatedMovie.ToResponse()); 
    }
    
    [Authorize(AuthConstants.AdminUserPolicyName)]
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
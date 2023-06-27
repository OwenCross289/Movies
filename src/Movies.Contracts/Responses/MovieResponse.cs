namespace Movies.Contracts.Responses;

public record MovieResponse(
    Guid Id,  
    string Title,
    string Slug,
    int YearOfRelease, 
    IEnumerable<string> Genres);
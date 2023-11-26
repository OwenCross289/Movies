namespace Movies.Contracts.Responses;

public record MovieResponse(
    Guid Id,
    string Title,
    string Slug,
    int YearOfRelease,
    float? Rating,
    int? UserRating,
    IEnumerable<string> Genres);
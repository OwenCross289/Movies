namespace Movies.Contracts.Requests;

public record UpdateMovieRequest(
    string Title,
    int YearOfRelease,
    IEnumerable<string> Genres);
namespace Movies.Contracts.Requests;

public record CreateMovieRequest(
    string Title, 
    int YearOfRelease, 
    IEnumerable<string> Genres);
namespace Movies.Contracts.Responses;

public record MoviesResponse(IEnumerable<MovieResponse> Items);
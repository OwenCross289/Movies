namespace Movies.Contracts.Responses;

public record ValidationFailureResponse(IEnumerable<ValidationResponse> Errors);

public record ValidationResponse(string PropertyName, string Message);

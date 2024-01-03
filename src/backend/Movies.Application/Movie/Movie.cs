using System.Text.RegularExpressions;

namespace Movies.Application.Movie;

public partial class Movie
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public required int YearOfRelease { get; set; }
    public string Slug => GenerateSlug();
    public float? Rating { get; set; }
    public int? UserRating { get; set; }
    public required List<string> Genres { get; init; } = new();

    private string GenerateSlug()
    {
        var sluggedTitle = RemoveSpecialCharsRegex()
            .Replace(Title, string.Empty)
            .ToLower().Replace(' ', '-');
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9a-zA-Z _-]", RegexOptions.NonBacktracking, 10)]
    private static partial Regex RemoveSpecialCharsRegex();
}
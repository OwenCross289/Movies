namespace Movies.Application.Database;

public static class Sql
{
    public static readonly string CreateMoviesTable = """
        create table if not exists movies (
            id UUID primary key,
            slug TEXT not null,
            title TEXT not null,
            yearofrelease integer not null);
        """;
    
    public static readonly string CreateSlugIndex = """
        create unique index concurrently if not exists movies_slug_idx
        on movies
        using btree(slug);
        """;
    
    public static readonly string CreateGenresTable = """
        create table if not exists genres (
            movieId UUID references movies (Id),
            name TEXT not null);
        """;

    public static readonly string CreateMovieWithoutGenre = """
        insert into movies (id, slug, title, yearofrelease)
        values (@Id, @Slug, @Title, @YearOfRelease)
        """;

    public static readonly string InsertGenresIntoMovie = """
        insert into genres (movieId, name)
        values (@MovieId, @Name)
        """;
    
    public static readonly string GetMovieById = """
        select * from movies where id = @id
        """;
    
    public static readonly string GetGenresForMovieById = """
        select name from genres where movieid = @id
        """;
    
    public static readonly string GetMovieBySlug = """
        select * from movies where slug = @slug
        """;
    
    public static readonly string GetAllMovies = """
        select m.*, string_agg(g.name, ',') as genres
        from movies m left join genres g on m.id = g.movieid
        group by id
        """;
    
    public static readonly string MovieExistsById = """
        select count(1) from movies where id = @id
        """;

    public static readonly string UpdateMovieById = """
        update movies set slug = @Slug, title = @Title, yearofrelease = @YearOfRelease
        where id = @Id
        """;
    
    public static readonly string DeleteGenresByMovieId = """
        delete from genres where movieid = @id
        """;
    
    public static readonly string DeleteMovieById = """
        delete from movies where id = @id
        """;
}
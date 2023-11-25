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
    
    public static readonly string CreateRatingsTable = """
        create table if not exists ratings (
            userid uuid,
            movieid uuid references movies (id), 
            rating integer not null,
            primary key (userid, movieid));
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
        select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
        from movies m 
        left join ratings r on m.id = r.movieid
        left join ratings myr on m.id = myr.movieid and myr.userid = @userId
        where id = @id
        group by id, userrating
        """;
    
    public static readonly string GetGenresForMovieById = """
        select name from genres where movieid = @id
        """;
    
    public static readonly string GetMovieBySlug = """
        select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
        from movies m 
        left join ratings r on m.id = r.movieid
        left join ratings myr on m.id = myr.movieid and myr.userid = @userId
        where slug = @slug
        group by id, userrating
        """;
    
    public static readonly string GetAllMovies = """
        select m.*,
               string_agg(distinct g.name, ',') as genres ,
               round(avg(r.rating), 1) as rating,
               myr.rating as userrating
        from movies m 
        left join genres g on m.id = g.movieid
        left join ratings r on m.id = r.movieid
        left join ratings myr on m.id = myr.movieid and myr.userid = @userId
        group by id, userrating
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
    
    public static readonly string GetRatingByMovieId = """
        select round (avg(r.rating), 1) from ratings r 
        where movieId = @movieId
        """;
    
    public static readonly string GetRatingByMovieIdAndUserid = """
        select round (avg(r.rating), 1),
               (select rating 
                from ratings
                where movieid = @movieId and userid = @userId
                limit 1)
        from ratings
        where movieId = @movieId
        """;
    
}
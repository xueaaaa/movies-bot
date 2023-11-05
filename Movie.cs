using Telegram.Bot.Types;

namespace MoviesBot
{
    /// <summary>
    /// A basic bot object to work with. Represents a description of the movie searched by the code
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// Movie code
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The title of the movie
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Movie Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Year of movie release
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Link to watch the movie
        /// </summary>
        public Uri Link { get; set; }
        /// <summary>
        /// Link to the cover of the movie
        /// </summary>
        public Uri Cover { get; set; }

        public Movie(int id, string name, string description, int year, Uri link, Uri cover)
        {
            Id = id;
            Name = name;
            Description = description;
            Year = year;
            Link = link;
            Cover = cover;
        }

        /// <summary>
        /// Adds the current object to the database
        /// </summary>
        public async Task<int> Add()
        {
            return await Program.Ctx.AddEntityAsync(this);
        }

        /// <summary>
        /// Deletes the current object from the database
        /// </summary>
        public async Task<int> Remove()
        {
            return await Program.Ctx.RemoveEntityAsync(this);
        }
    }
}

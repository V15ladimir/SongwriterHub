namespace Songwriter.Models {

    public class Song {

        public required int Id { get; set; }

        public required string Artist { get; set; }

        public required string Title { get; set; }

        public required string Icon { get; set; }

        public required string Album { get; set; }

        public required string Genre { get; set; }

        public required int Likes { get; set; }

        public required IEnumerable<string> Lyrics { get; set; }
    }
}

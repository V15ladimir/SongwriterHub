namespace Songwriter.Configuration {

    public class FakerConfig {
        public int LyricsLineCount { get; set; }
        public int LyricsMinWords { get; set; } 
        public int LyricsMaxWords { get; set; } 
        public string TitleWords { get; set; } = string.Empty;
    }
}

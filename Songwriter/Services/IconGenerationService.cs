using Songwriter.Models;

namespace Songwriter.Services {

    public class IconGenerationService : IIconGenerationService {
        public async Task<string> CoverIconAsync(Song song) {
            using var http = new HttpClient();
            var svg = await http.GetStringAsync(song.Icon);
            return svg.Replace("</svg>",
                $@"<rect x='0' y='40' width='100%' height='20%' fill='rgba(0,0,0,0.7)' />
                <text x='10' y='50' font-size='5' fill='white' font-family='Arial' font-weight='bold'>{song.Title}</text>
                <text x='10' y='58' font-size='3.5' fill='#cccccc' font-family='Arial'>{song.Artist}</text></svg>");
        }
    }
}

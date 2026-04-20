using Bogus;
using Microsoft.Extensions.Options;
using Songwriter.Configuration;
using Songwriter.Models;

namespace Songwriter.Services {

    public class SongGegerationService(IOptions<AppSettings> settings) : ISongGenerationService {
        public IEnumerable<Song> GenerateSongs(ulong seed, string locale, double avgLikes, int page) {
            var pageSeed = seed ^ (ulong)page;
            var randomLikes = new XoshiroRandomAdapter(pageSeed);
            
            Randomizer.Seed = new XoshiroRandomAdapter(pageSeed);
            return new Faker<Song>(locale)
                .RuleFor(x => x.Id, (y, z) => CalculateIndex(y, page))
                .RuleFor(x => x.Artist, (y, z) => GenerateFirstName(y))
                .RuleFor(x => x.Title, (y, z) => GenerateSentence(y))
                .RuleFor(x => x.Album, (y, z) => GenerateSentence(y))
                .RuleFor(x => x.Icon, GenerateIcon)
                .RuleFor(x => x.Genre, (y, z) => GenerateMusicGenre(y))
                .RuleFor(x => x.Likes, (y, z) => GenerateLikes(randomLikes, avgLikes))
                .RuleFor(x => x.Lyrics, (y, z) => GenerateLyrics(y))
                .Generate(settings.Value.Paging.PageSize);
        }

        private int CalculateIndex(Faker faker, int page) {
            faker.IndexVariable++;
            return (page - 1) * settings.Value.Paging.PageSize + faker.IndexVariable;
        }

        private int GenerateLikes(Random random, double avgLikes) {
            var min = (int)Math.Floor(avgLikes);
            var max = (int)Math.Ceiling(avgLikes);
            if(min == max)
                return min;
            return random.NextDouble() < avgLikes - min ? max : min;
        }

        private string GenerateFirstName(Faker faker) => faker.Name.FirstName();

        private string GenerateMusicGenre(Faker faker) => faker.Music.Genre();

        private string GenerateSentence(Faker faker) => faker.Lorem.Sentence(1, 2).TrimEnd('.');

        private string GenerateIcon(Faker faker, Song song) => faker.DiceBear().Shapes(
            settings.Value.Icon.IconFormat, 
            song.Id.ToString(), 
            settings.Value.Icon.IconSize, 
            settings.Value.Icon.IconScale);

        private IEnumerable<string> GenerateLyrics(Faker faker) => Enumerable.Range(0, 20)
            .Select(x => faker.Lorem.Sentence(settings.Value.Faker.LyricsMinWords, settings.Value.Faker.LyricsMaxWords));
    }
}

using Songwriter.Models;

namespace Songwriter.Services {

    public interface ISongGenerationService {
        public IEnumerable<Song> GenerateSongs(ulong seed, string locale, double avgLikes, int page);
    }
}

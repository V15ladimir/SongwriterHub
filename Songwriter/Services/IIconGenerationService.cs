using Songwriter.Models;

namespace Songwriter.Services {

    public interface IIconGenerationService {
        Task<string> CoverIconAsync(Song song);
    }
}

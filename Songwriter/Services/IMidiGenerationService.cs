namespace Songwriter.Services {

    public interface IMidiGenerationService {
        byte[] GenerateComposition(int id, ulong seed);
    }
}

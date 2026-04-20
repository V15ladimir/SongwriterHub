namespace Songwriter.Configuration {

    public class DefaultsConfig {
        public ulong Seed { get; set; }
        public string Language { get; set; } = string.Empty;
        public double AvgLikes { get; set; }
        public int Page { get; set; }
    }
}

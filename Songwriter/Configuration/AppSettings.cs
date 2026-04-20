namespace Songwriter.Configuration {

    public class AppSettings {
        public PagingConfig Paging { get; set; } = new();
        public FakerConfig Faker { get; set; } = new();
        public IconConfig Icon { get; set; } = new();
        public DefaultsConfig Defaults { get; set; } = new();
    }
}

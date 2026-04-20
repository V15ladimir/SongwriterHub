using Redzen.Random;

namespace Songwriter.Services {

    public class XoshiroRandomAdapter(ulong seed) : Random {
        private readonly Xoshiro256StarStarRandom _rng = new(seed);

        public override int Next() => _rng.Next();

        public override int Next(int maxValue) => _rng.Next(maxValue);

        public override int Next(int minValue, int maxValue) => _rng.Next(minValue, maxValue);

        public override void NextBytes(byte[] buffer) => _rng.NextBytes(buffer);

        protected override double Sample() => _rng.NextDouble();
    }
}

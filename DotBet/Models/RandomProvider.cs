using DotBet.Interfaces;

namespace DotBet.Models
{
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random = new Random();

        public double NextDouble()
        {
            // return a number which is between 0.00 and 1.00 inclusive!
            return this._random.Next(0,101) / 100.0;
        }
    }
}

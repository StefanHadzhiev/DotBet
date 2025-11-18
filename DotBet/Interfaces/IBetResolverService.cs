using DotBet.Models;

namespace DotBet.Interfaces
{
    public interface IBetResolverService
    {
        public Result<decimal> Resolve(decimal amount, decimal balance);
    }
}

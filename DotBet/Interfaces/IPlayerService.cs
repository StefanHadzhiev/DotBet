using DotBet.Models;

namespace DotBet.Interfaces
{
    public interface IPlayerService
    {
        public decimal Balance { get; }
        public Result<decimal> Bet(decimal amount);
        public Result<decimal> Deposit(decimal amount);
        public Result<decimal> Withdraw(decimal amount); 
    }
}

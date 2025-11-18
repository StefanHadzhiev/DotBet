using DotBet.Interfaces;
using DotBet.Models;
using DotBet.Shared;
using Microsoft.Extensions.Options;

namespace DotBet.Services
{
    public class PlayerService : IPlayerService
    {
        private decimal _balance;

        private IBetResolverService _betResolverService;

        private readonly BetSettings _betSettings;

        public decimal Balance
        { 
            get => Math.Round(_balance, 2);
            private set => _balance = value;   
        }

        public PlayerService(IBetResolverService betResolverService, IOptions<BetSettings> betSettings)
        {
            this._betResolverService = betResolverService;
            this._betSettings = betSettings.Value;
        }

        public Result<decimal> Bet(decimal amount)
        {
            // Validation checking if the current bet amount is within accepted range
            if (amount >= _betSettings.BetLowerBound && amount <= _betSettings.BetUpperBound)
            {
                // Validation checking if the current balance is greater than the attempted bet amount
                if (amount > this.Balance)
                {
                    return Result<decimal>.Failure(String.Format(Constants.InsufficientFundsForBet, this.Balance));
                }

                var result = this._betResolverService.Resolve(amount, this.Balance);
                this.Balance = result.Value;
                return result;
            }

            return Result<decimal>.Failure(String.Format(Constants.EnterValidRangeError, this._betSettings.BetLowerBound, this._betSettings.BetUpperBound));
        }

        public Result<decimal> Deposit(decimal amount)
        { 
            this.Balance += amount;
            return Result<decimal>.Success(this.Balance, $"{Constants.SuccessfulOperation} \n {String.Format(Constants.CurrentBalanceIs, this.Balance)}");
        }

        public Result<decimal> Withdraw(decimal amount)
        {
            if (this.Balance >= amount)
            {
                this.Balance -= amount;
                return Result<decimal>.Success(this.Balance, $"{Constants.SuccessfulOperation} \n {String.Format(Constants.CurrentBalanceIs, this.Balance)}");
            }

            return Result<decimal>.Failure(String.Format(Constants.InsufficientFunds, this.Balance));
        }
    }
}

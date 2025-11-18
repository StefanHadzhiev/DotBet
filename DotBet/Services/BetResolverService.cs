using DotBet.Interfaces;
using DotBet.Models;
using DotBet.Shared;
using Microsoft.Extensions.Options;

namespace DotBet.Services
{
    public class BetResolverService : IBetResolverService
    {
        private IRandomProvider _randomProvider;

        private BetSettings _betSettings; 

        public BetResolverService(IRandomProvider randomProvider, IOptions<BetSettings> betSettings)
        {
            this._randomProvider = randomProvider;
            this._betSettings = betSettings.Value;
        }
        public Result<decimal> Resolve(decimal amount, decimal balance)
        {
            double roll = this._randomProvider.NextDouble();
            decimal winAmount = 0;

            if (roll < (double)this._betSettings.LossChance) {
                winAmount = 0;
            } 
            else if(roll < (double)this._betSettings.LossChance + (double)this._betSettings.SmallWinChance)
            {
                double multiplier = (double)this._betSettings.SmallWinLowerBound + this._randomProvider.NextDouble();
                winAmount = amount * (decimal)multiplier;
            } 
            else
            {
                double multiplier = (double)this._betSettings.BigWinLowerBound + this._randomProvider.NextDouble() * ((double)this._betSettings.BigWinUpperBound - (double)this._betSettings.BigWinLowerBound);
                winAmount = amount * (decimal)multiplier;
            }

            var newBalance = balance - amount + winAmount;

            // determine win or lose depending on the amount and set success message 

            string message = String.Empty;

            if(winAmount == 0)
            {
                message = String.Format(Constants.LoseMessage, newBalance);
            } else
            {
                message = String.Format(Constants.WinMessage, newBalance - balance, newBalance);
            }

            return Result<decimal>.Success(newBalance, message);
        }
    }
}

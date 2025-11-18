using DotBet.Interfaces;
using DotBet.Models;
using DotBet.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace DotBet.Tests
{
    public class BetResolverServiceTests
    {
        private readonly Mock<IRandomProvider> _mockRandomProvider;
        private readonly BetResolverService _betResolverService;
        private readonly BetSettings _betSettings;

        public BetResolverServiceTests()
        {
            _mockRandomProvider = new Mock<IRandomProvider>();
            _betSettings = new BetSettings
            {
                LossChance = 0.5m,
                SmallWinChance = 0.4m,
                SmallWinLowerBound = 1m,
                BigWinLowerBound = 2m,
                BigWinUpperBound = 10m
            };

            var options = Options.Create(_betSettings);
            _betResolverService = new BetResolverService(_mockRandomProvider.Object, options);
        }

        [Fact]
        public void Resolve_ShouldLose_WhenRollIsBelowLossChance()
        {
            // Arrange
            _mockRandomProvider.Setup(r => r.NextDouble()).Returns(0.3);
            decimal betAmount = 100m;
            decimal balance = 500m;

            // Act
            var result = _betResolverService.Resolve(betAmount, balance);

            // Assert
            Assert.Equal(balance - betAmount, result.Value);
            Assert.Contains("No luck", result.Message); 
        }

        [Fact]
        public void Resolve_ShouldSmallWin_WhenRollIsInSmallWinRange()
        {
            // Arrange
            double roll = (double)_betSettings.LossChance + 0.1;
            _mockRandomProvider.SetupSequence(r => r.NextDouble())
                               .Returns(roll) 
                               .Returns(0.5);

            decimal betAmount = 100m;
            decimal balance = 500m;

            // Act
            var result = _betResolverService.Resolve(betAmount, balance);

            double multiplier = (double)_betSettings.SmallWinLowerBound + 0.5; 
            decimal expectedWinAmount = betAmount * (decimal)multiplier;
            decimal expectedBalance = balance - betAmount + expectedWinAmount;

            // Assert
            Assert.Equal(expectedBalance, result.Value);
            Assert.Contains("Congrats", result.Message);
        }

        [Fact]
        public void Resolve_ShouldBigWin_WhenRollIsAboveSmallWinRange()
        {
            // Arrange
            double roll = (double)_betSettings.LossChance + (double)_betSettings.SmallWinChance + 0.1;
            _mockRandomProvider.SetupSequence(r => r.NextDouble())
                               .Returns(roll)  
                               .Returns(0.3);   

            decimal betAmount = 100m;
            decimal balance = 500m;

            // Act
            var result = _betResolverService.Resolve(betAmount, balance);

            double multiplier = (double)_betSettings.BigWinLowerBound + 0.3 * ((double)_betSettings.BigWinUpperBound - (double)_betSettings.BigWinLowerBound);
            decimal expectedWinAmount = betAmount * (decimal)multiplier;
            decimal expectedBalance = balance - betAmount + expectedWinAmount;

            // Assert
            Assert.Equal(expectedBalance, result.Value);
            Assert.Contains("Congrats", result.Message);
        }
    }

}

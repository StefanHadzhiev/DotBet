using DotBet.Interfaces;
using DotBet.Models;
using DotBet.Services;
using DotBet.Shared;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DotBet.Tests
{
  

    public class PlayerServiceTests
    {
        private readonly Mock<IBetResolverService> _mockBetResolver;
        private readonly PlayerService _playerService;
        private readonly BetSettings _betSettings;

        public PlayerServiceTests()
        {
            _mockBetResolver = new Mock<IBetResolverService>();
            _betSettings = new BetSettings
            {
                BetLowerBound = 10m,
                BetUpperBound = 1000m
            };

            var options = Options.Create(_betSettings);
            _playerService = new PlayerService(_mockBetResolver.Object, options);
        }

        [Fact]
        public void Deposit_ShouldIncreaseBalance()
        {
            // Arrange
            decimal depositAmount = 100m;

            // Act
            var result = _playerService.Deposit(depositAmount);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(100m, _playerService.Balance);
        }

        [Fact]
        public void Withdraw_ShouldDecreaseBalance_WhenSufficientFunds()
        {
            // Arrange
            _playerService.Deposit(200m);

            // Act
            var result = _playerService.Withdraw(150m);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(50m, _playerService.Balance);
        }

        [Fact]
        public void Withdraw_ShouldFail_WhenInsufficientFunds()
        {
            // Arrange
            _playerService.Deposit(50m);

            // Act
            var result = _playerService.Withdraw(100m);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Message, String.Format(Constants.InsufficientFunds, 50m));
        }

        [Fact]
        public void Bet_ShouldReturnFailure_WhenAmountBelowLowerBound()
        {
            // Act
            var result = _playerService.Bet(-1m);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Message, String.Format(Constants.EnterValidRangeError, _betSettings.BetLowerBound, _betSettings.BetUpperBound));
        }

        [Fact]
        public void Bet_ShouldReturnFailure_WhenAmountAboveUpperBound()
        {
            // Act
            var result = _playerService.Bet(2000m);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Message, String.Format(Constants.EnterValidRangeError, _betSettings.BetLowerBound, _betSettings.BetUpperBound));
        }

        [Fact]
        public void Bet_ShouldReturnFailure_WhenInsufficientFunds()
        {
            // Arrange
            _playerService.Deposit(10m);

            // Act
            var result = _playerService.Bet(11m);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Message, String.Format(Constants.InsufficientFundsForBet, 10));
        }
    }

}

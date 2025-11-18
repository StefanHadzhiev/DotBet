using DotBet.Interfaces;
using DotBet.Models;
using DotBet.Services;
using DotBet.Shared;
using Microsoft.Extensions.Options;
using Moq;

namespace DotBet.Tests
{
    public class GameServiceTests
    {
        private readonly Mock<IInteractorService> _interactorMock;
        private readonly Mock<IPlayerService> _playerServiceMock;
        private readonly Mock<IParserService> _parserServiceMock;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            _interactorMock = new Mock<IInteractorService>();
            _playerServiceMock = new Mock<IPlayerService>();
            _parserServiceMock = new Mock<IParserService>();

            var betSettings = Options.Create(new BetSettings());

            _gameService = new GameService(
                _interactorMock.Object,
                _playerServiceMock.Object,
                _parserServiceMock.Object,
                betSettings
            );
        }

        [Theory]
        [InlineData("exit")]
        [InlineData("bet 10")]
        [InlineData("deposit 5")]
        [InlineData("withdraw 7")]
        public void ValidateInput_ReturnsTrue_ForValidInputs(string input)
        {
            string[]? split = input.ToLower().TrimEnd().Split(" ");
            var result = _gameService.ValidateInput(split);
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("bet 10 extra" )]
        [InlineData("invalid")]
        public void ValidateInput_ReturnsFalse_ForInvalidInputs(string input)
        {
            string[]? split = input.ToLower().TrimEnd().Split(" ");
            var result = _gameService.ValidateInput(split);
            Assert.False(result);
        }


        [Fact]
        public void ProvideInstructions_WritesExpectedLines()
        {
            _gameService.ProvideInstructions();

            _interactorMock.Verify(i => i.WriteOutput("Welcome to DotBet!"), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput("Please enter command in the format [Command] [Amount] or \"exit\""), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput("Available commands:"), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput("Deposit [amount]"), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput("Withdraw [amount]"), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput("Bet [amount]"), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput("Exit"), Times.Once);
        }

        [Fact]
        public void HandleBetCommand_CallsBet_AndWritesOutput()
        {
            decimal balance = 100;
            decimal newBalance = 101;

            var result = Result<decimal>.Success(newBalance, "Congrats");
            _playerServiceMock.Setup(s => s.Bet(10)).Returns(result);

            _gameService.HandleBetCommand(10);

            _playerServiceMock.Verify(s => s.Bet(10), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput(result.Message), Times.Once);
        }

        [Fact]
        public void HandleDepositCommand_CallsDeposit_AndWritesOutput()
        {
            var result = Result<decimal>.Success(50, "Operation was successful.");
            _playerServiceMock.Setup(s => s.Deposit(50)).Returns(result);

            _gameService.HandleDepositCommand(50);

            _playerServiceMock.Verify(s => s.Deposit(50), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput(result.Message), Times.Once);
        }

        [Fact]
        public void HandleWithdrawCommand_CallsWithdraw_AndWritesOutput()
        {
            var result = Result<decimal>.Success(50, "Operation was successful.");
            _playerServiceMock.Setup(s => s.Withdraw(20)).Returns(result);

            _gameService.HandleWithdrawCommand(20);

            _playerServiceMock.Verify(s => s.Withdraw(20), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput(result.Message), Times.Once);
        }

        [Fact]
        public void Start_Stops_OnExitCommand()
        {
            _interactorMock.SetupSequence(i => i.ReadInput())
                .Returns("exit");

            _gameService.Start();

            _interactorMock.Verify(i => i.WriteOutput(Constants.FarewellMessage), Times.Once);
        }

        [Fact]
        public void Start_InvalidInput_ShowsError()
        {
            _interactorMock.SetupSequence(i => i.ReadInput())
                .Returns("invalid")
                .Returns("exit");

            _gameService.Start();

            _interactorMock.Verify(i => i.WriteOutput(Constants.EnterValidCommandError), Times.Once);
        }

        [Fact]
        public void Start_ValidBetCommand_FlowsCorrectly()
        {
            // Arrange input
            _interactorMock.SetupSequence(i => i.ReadInput())
                .Returns("bet 10")
                .Returns("exit");

            // Parser should parse "10" successfully
            _parserServiceMock
                .Setup(p => p.Parse("10", out It.Ref<decimal>.IsAny))
                .Callback(new ParseCallback((string s, out decimal amount) => amount = 10))
                .Returns(true);

            _playerServiceMock
                .Setup(p => p.Bet(10))
                .Returns(Result<decimal>.Success(10, "Bet OK"));

            // Act
            _gameService.Start();

            // Assert
            _playerServiceMock.Verify(p => p.Bet(10), Times.Once);
            _interactorMock.Verify(i => i.WriteOutput("Bet OK"), Times.Once);
        }


        private delegate void ParseCallback(string s, out decimal amount);
    }
}

using DotBet.Interfaces;
using DotBet.Models;
using DotBet.Shared;
using Microsoft.Extensions.Options;

namespace DotBet.Services
{
    public class GameService : IGameService
    {
        private IInteractorService _interactor;

        private IPlayerService _playerService;

        private IParserService _parserService;

        private readonly BetSettings _betSettings;
        public GameService(IInteractorService interactor, IPlayerService playerService, IParserService parserService, IOptions<BetSettings> betSettings)
        {
            _interactor = interactor;
            _playerService = playerService;
            _parserService = parserService;
            _betSettings = betSettings.Value;
        }

        public void Start()
        {
            ProvideInstructions();

            while (true)
            {
                var input = this._interactor.ReadInput();
                var split = input.ToLower().TrimEnd().Split(" ");

                // Check for exit command
                if (split.Length == 1 && split[0] == Constants.ExitCommand)
                {
                    break;
                }
                // Validate user input
                else if (!ValidateInput(split))
                {
                    _interactor.WriteOutput(Constants.EnterValidCommandError);
                    FileLogger.Log(Constants.EnterValidCommandError);
                    continue;
                }
                else
                {
                    var command = split[0];
                    if (!this._parserService.Parse(split[1], out decimal amount))
                    {
                        // Input amount was not in correct format.
                        _interactor.WriteOutput(Constants.EnterValidCommandError);
                        FileLogger.Log(Constants.EnterValidCommandError);
                        continue;
                    }
                    else
                    {
                        // Happy Path - amount was parsed successfully.
                        switch (command)
                        {
                            case Constants.BetCommand:
                                this.HandleBetCommand(amount);
                                continue;
                            case Constants.DepositCommand:
                                this.HandleDepositCommand(amount);
                                continue;
                            case Constants.WithdrawCommand:
                                HandleWithdrawCommand(amount);
                                continue;
                            default:
                                _interactor.WriteOutput(Constants.EnterValidCommandError);
                                FileLogger.Log(Constants.EnterValidCommandError);
                                continue;
                        }
                    }
                }
            }

            _interactor.WriteOutput(Constants.FarewellMessage);
        }

        public void ProvideInstructions()
        {
            _interactor.WriteOutput("Welcome to DotBet!");
            _interactor.WriteOutput("Please enter command in the format [Command] [Amount] or \"exit\"");
            _interactor.WriteOutput("Available commands:");
            _interactor.WriteOutput("Deposit [amount]");
            _interactor.WriteOutput("Withdraw [amount]");
            _interactor.WriteOutput("Bet [amount]");
            _interactor.WriteOutput("Exit");
        }

        public bool ValidateInput(string[]? input)
        {
            if (input.Length > 2 ||
                   (input.Length == 1 && input[0] != Constants.ExitCommand) ||
                   (input.Length == 0))
            {
                return false;
            }

            return true;
        }

        public void HandleBetCommand(decimal amount)
        {
            var result = this._playerService.Bet(amount);
            _interactor.WriteOutput(result.Message);
            FileLogger.Log(result.Message);
        }

        public void HandleDepositCommand(decimal amount)
        {
            var result = this._playerService.Deposit(amount);
            _interactor.WriteOutput(result.Message);
            FileLogger.Log(result.Message);
        }

        public void HandleWithdrawCommand(decimal amount)
        {
            var result = this._playerService.Withdraw(amount);
            _interactor.WriteOutput(result.Message);
            FileLogger.Log(result.Message);
        }
    }
}

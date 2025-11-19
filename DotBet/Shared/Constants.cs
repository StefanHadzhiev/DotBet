namespace DotBet.Shared
{
    public static class Constants
    {
        public const string SuccessfulOperation = "Operation was successful.";
        public const string CurrentBalanceIs = "Current Balance: ${0}.";
        public const string InsufficientFunds = "Insufficient Funds! You can withdraw up to ${0}";
        public const string WinMessage = "Congrats - you won ${0}! Your current balance is: ${1}.";
        public const string LoseMessage = "No luck this time! Your current balance is ${0}.";
        public const string EnterValidRangeError = "Please enter an amount between ${0} and ${1}";
        public const string EnterValidCommandError = "Please enter a valid command in the format [Command] [Amount] or \"exit\".";
        public const string InsufficientFundsForBet = "Insufficient Funds! Current balance: ${0}"; 
        public const string FarewellMessage = "Goodbye, thank you for playing!"; 

        public const string ExitCommand = "exit";
        public const string DepositCommand = "deposit";
        public const string WithdrawCommand = "withdraw";
        public const string BetCommand = "bet";
    }
}

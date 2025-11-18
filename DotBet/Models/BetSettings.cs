namespace DotBet.Models
{
    public class BetSettings
    {
        public decimal BetLowerBound { get; set; }
        public decimal BetUpperBound { get; set; }
        public decimal LossChance { get; set; }
        public decimal SmallWinChance { get; set; }
        public decimal SmallWinLowerBound { get; set; }
        public decimal BigWinLowerBound { get; set; }
        public decimal BigWinUpperBound { get; set; }
    }
}

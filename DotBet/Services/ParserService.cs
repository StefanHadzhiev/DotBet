using DotBet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBet.Services
{
    public class ParserService : IParserService
    {
        public bool Parse(string input, out decimal amount)
        {
            if (decimal.TryParse(input, out amount)) return true;

            return false;
        }
    }
}

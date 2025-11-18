using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBet.Interfaces
{
    public interface IParserService
    {
        public bool Parse(string input, out decimal amount);
    }
}

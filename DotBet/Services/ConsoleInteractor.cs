using DotBet.Interfaces;

namespace DotBet.Services
{
    public class ConsoleInteractor : IInteractorService
    {
        public string ReadInput()
        {
            return Console.ReadLine();
        }

        public void WriteOutput(string message)
        {
            Console.WriteLine(message);
        }
    }
}

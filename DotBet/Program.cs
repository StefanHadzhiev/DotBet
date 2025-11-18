using DotBet.Interfaces;
using DotBet.Models;
using DotBet.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotBet
{
    public class Program
    {
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services
              .AddSingleton<IInteractorService, ConsoleInteractor>()
              .AddSingleton<IGameService, GameService>()
              .AddSingleton<IPlayerService, PlayerService>()
              .AddSingleton<IBetResolverService, BetResolverService>()
              .AddSingleton<IRandomProvider, RandomProvider>()
              .AddSingleton<IParserService, ParserService>();

            builder.Services.Configure<BetSettings>(
                builder.Configuration.GetSection("BetSettings"));


            var host = builder.Build();

            var gameService = host.Services.GetRequiredService<IGameService>();

            gameService.Start();    
        }
    }
}

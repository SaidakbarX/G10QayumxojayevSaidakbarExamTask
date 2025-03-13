using ExamBot.Bll.Services;
using ExamBot.Dal;
using Microsoft.Extensions.DependencyInjection;

namespace ExamBot;

internal class Program
{
    static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddScoped<IUserServices, UserServices>();
        serviceCollection.AddScoped<IUserInfoServices, UserInfoServices>();
        serviceCollection.AddSingleton<MainContext>();
        serviceCollection.AddSingleton<BotLogic>();


        var serviceProvider = serviceCollection.BuildServiceProvider();

        var botListenerService = serviceProvider.GetRequiredService<BotLogic>();
        await botListenerService.StartBot();

        Console.ReadKey();

    }
}

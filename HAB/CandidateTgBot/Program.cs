// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


using var cts = new CancellationTokenSource();


Console.ReadLine();
cts.Cancel();





enum Step
{
    None,
    WaitingForAspNet,
    WaitingForRussian
}
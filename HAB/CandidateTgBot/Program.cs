// See https://aka.ms/new-console-template for more information

using var cts = new CancellationTokenSource();


Console.ReadLine();
cts.Cancel();





namespace CandidateTgBot
{
    enum Step
    {
        None,
        WaitingForAspNet,
        WaitingForRussian
    }
}
using Rubika.Package.Bot;

IBot _bot = new Bot("auth");

await _bot.CreateBotAsync((message) =>
{
    Console.WriteLine(message.Id);
}, "gapToken");
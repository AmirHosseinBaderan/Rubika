using Rubika.Package.Bot;

IBot _bot = new Bot("krirsqxchedzqgmkjvxhhujyysgshkbj");

await _bot.CreateBotAsync((message) =>
{
    Console.WriteLine(message.Id);
}, "g0qSWk0ee31cd431a7ac489106fcf62a");
using Rubika.Package.Bot;
using Rubika.Package.Model;

string _auth = ".";
string _gapToken = ".";

IBot _bot = new Bot(_auth);

await _bot.CreateBotAsync(async (message) =>
{
    if (message.Status == ActionStatus.Success)
    {
        Int32 unixTimestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        var chats = await _bot.GetChatsUpdatesAsync(unixTimestamp.ToString());
        if (chats.Status == ActionStatus.Success)
            foreach (var chat in chats.Chats)
                Console.WriteLine(chat.ObjectGuid);

    }
    else
        Console.WriteLine("exception");

}, _gapToken);
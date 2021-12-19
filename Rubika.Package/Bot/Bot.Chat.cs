namespace Rubika.Package.Bot;

public partial class Bot : IBot, IDisposable
{
    public async Task SeenCahtAsync(SeenChat seenChat)
           => await Task.Run(async () =>
           {
               string gapToken = seenChat.GapToken ?? _gapToken;

               JObject input = JObject.Parse("{\"seen_list\" : {\"" + gapToken + "\":\"" + seenChat.MessageId + "\"}}");
               string v4Data = await CreateDataV4Async(input.ToString(), "seenChats", _auth);
               await _api.SendRequestAsync(_url, v4Data.GetBytes());
           });

    public async Task<GetUpdatesChats> GetChatsUpdatesAsync(string timeStamp = null)
            => await Task.Run(async () =>
            {
                if (timeStamp == null)
                    timeStamp = ((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                try
                {
                    JObject input = new()
                    {
                        { "state", timeStamp }
                    };
                    string v4Data = await CreateDataV4Async(input.ToString(), "getChatsUpdates", _auth);
                    string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
                    JObject response = await _api.ConvertToJObjectAsync(request);
                    JArray chats = JArray.Parse(response["chats"].ToString());
                    List<Chat> result = new();
                    foreach (JToken chat in chats)
                        result.Add(CreateChat(chat));
                    return new GetUpdatesChats(ActionStatus.Success, result);
                }
                catch
                {
                    return new GetUpdatesChats(ActionStatus.Exception, null);
                }
            });

    public async Task<JObject> GetObjectByUserNameAsync(string userName)
        => await Task.Run(async () =>
        {
            string json = new JObject
            {
                {"username",userName }
            }.ToString();
            string v4Data = await CreateDataV4Async(json, "getObjectByUsername", _auth);
            string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
            return await _api.ConvertToJObjectAsync(request);
        });

    public async Task<GetChannelByUserName> GetChannelByUserNameAsync(string userName)
        => await Task.Run(async () =>
        {
            JObject getObject = await GetObjectByUserNameAsync(userName);
            if (!getObject.ContainsKey("err"))
            {
                Channel channel = CreateChannel(getObject["channel"]);
                return new GetChannelByUserName(channel)
                {
                    Status = ActionStatus.Success,
                    Chat = CreateChat(getObject["chat"]),
                    Exist = bool.Parse(getObject["exist"]?.ToString() ?? "false"),
                    Type = getObject["type"]?.ToString()
                };
            }
            return new GetChannelByUserName(null) { Status = ActionStatus.Exception };
        });
}

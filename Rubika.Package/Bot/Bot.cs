namespace Rubika.Package.Bot;

/// <summary>
/// Rubika Bot Services Implement <see cref="IBot"/>
/// </summary>
public partial class Bot : IBot, IDisposable
{
    public async Task CreateBotAsync(Action<GetMessage> newMessage, string gapToken)
        => await Task.Run(() =>
        {
            _gapToken = gapToken;
            _onGetMessage = newMessage;
            new Thread(GetMessage).Start();
        });

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void GetMessage()
    {
        string minId = GetMinIdAsync(_gapToken).Result;
        JObject json = new()
        {
            { "limit", 10 },
            { "min_id", minId },
            { "object_guid", _gapToken },
            { "sort", "FromMin" }
        };

        while (true)
        {
            try
            {
                string dataReq = CreateDataV4Async(json.ToString(), "getMessages", _auth).Result;
                string req = _api.SendRequestAsync(_url, dataReq.GetBytes()).Result;
                JObject decDataEnc = _api.ConvertToJObjectAsync(req).Result;
                if (!decDataEnc.ContainsKey("err"))
                {
                    JArray messages = JArray.Parse(decDataEnc["messages"].ToString());
                    if (messages.Count > 0)
                    {
                        foreach (JObject message in messages)
                            if (!_messages.Any(m => m.Id == message["message_id"].ToString()))
                            {
                                Message newMessage = CreateMessage(message);
                                _onGetMessage(new(ActionStatus.Success, newMessage));
                                _messages.Add(newMessage);
                            }

                        json.Remove("min_id");
                        json.Add("min_id", _messages[^1].Id);
                    }
                }
            }
            catch
            {
                _onGetMessage(new(ActionStatus.Success, null));
            }

            Thread.Sleep(600);
        }
    }

    public async Task<string> GetMinIdAsync(string gapToken)
        => await Task.Run(async () =>
        {
            GetGroupInfo groupInfo = await GetGroupInfoFromTokenAsync(gapToken);
            return groupInfo.Status == ActionStatus.Success ? groupInfo.Chat.LastMessage.Id : "";
        });

    public async Task<UserInof> GetUserInfoAsync(string userToken)
    => await Task.Run(async () =>
    {
        string createData = await CreateDataV4Async("{\"user_guid\":\"" + userToken + "\"}", "getUserInfo", _auth);
        string request = await _api.SendRequestAsync(_url, createData.GetBytes());
        JObject resObject = await _api.ConvertToJObjectAsync(request);
        JToken user = resObject["user"];
        return new UserInof()
        {
            Name = user["first_name"]?.ToString(),
            LastName = user["last_name"]?.ToString(),
            Bio = user["bio"]?.ToString(),
            UserName = user["username"]?.ToString(),
            UserGuid = user["user_guid"]?.ToString()
        };
    });

    public async Task<string> GetGuidFromUserNameAsync(string userName)
        => await Task.Run(async () =>
        {
            string v4Data = await CreateDataV4Async("{\"username\":\"" + userName + "\"}", "getObjectByUsername", _auth);
            string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
            JObject resJson = await _api.ConvertToJObjectAsync(request);
            return resJson["user"]["user_guid"].ToString();
        });

    public async Task<DeleteMessage> DeleteMessageAsync(IEnumerable<string> messagesId, string gapToken)
        => await Task.Run(async () =>
        {
            string msgIds = string.Join(",", messagesId);
            string json = new JObject
            {
                {"message_ids",JArray.Parse($"[{msgIds}]") },
                {"object_guid",gapToken },
                {"type","Global" }
            }.ToString();
            string v4Data = await CreateDataV4Async(json, "deleteMessages", _auth);
            string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
            JObject response = await _api.ConvertToJObjectAsync(request);
            return CreateDeleteMessage(response);
        });

    public async Task<SendMessage> SendMessageAsync(string text, string replyId, string gapToken)
            => await Task.Run(async () =>
            {
                JObject json = new()
                {
                    { "is_mute", false },
                    { "object_guid", gapToken },
                    { "rnd", new Random().Next(100000000, 999999999) },
                    { "text", text }
                };

                if (replyId != null)
                    json.Add("reply_to_message_id", replyId);

                string v4Data = await CreateDataV4Async(json.ToString(), "sendMessage", _auth);
                string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
                JObject response = await _api.ConvertToJObjectAsync(request);
                return CreateSendMessage(response);
            });

    public async Task EditMessageAsync(string text, string messageId, string gapToken)
            => await Task.Run(async () =>
            {
                JObject json = new()
                {
                    { "is_mute", false },
                    { "object_guid", gapToken },
                    { "rnd", new Random().Next(100000000, 999999999) },
                    { "text", text },
                    { "message_id", messageId }
                };
                string v4Data = await CreateDataV4Async(json.ToString(), "editMessage", _auth);
                await _api.SendRequestAsync(_url, v4Data.GetBytes());
            });

    public async Task SendLocationAsync(double lat, double lon, string gapToken)
        => await Task.Run(async () =>
        {
            JObject json = new()
            {
                { "is_mute", false },
                { "object_guid", gapToken },
                { "rnd", new Random().Next(100000000, 999999999) },
                { "location", JObject.Parse("{\"latitude\":" + lat + ",\"longitude\":" + lon + "}") }
            };
            string data = await CreateDataV4Async(json.ToString(), "sendMessage", _auth);
            await _api.SendRequestAsync(_url, data.GetBytes());
        });

    public async Task<Message> GetMessageByIdAsync(string messageId, string gapToken)
            => await Task.Run(async () =>
            {
                string v4Data = await CreateDataV4Async("{\"message_ids\":[\"" + messageId + "\"],\"object_guid\":\"" + gapToken + "\"}", "getMessagesByID", _auth);
                string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
                JObject data = await _api.ConvertToJObjectAsync(request);
                JToken message = JArray.Parse(data["messages"].ToString())[0];
                return CreateMessage(message.ToString());
            });

    public async Task RemoveUserAsync(string userToken, string gapToken)
            => await Task.Run(async () =>
            {
                JObject json = new()
                {
                    { "action", "Set" },
                    { "group_guid", gapToken },
                    { "member_guid", userToken },
                };
                string v4Data = await CreateDataV4Async(json.ToString(), "banGroupMember", _auth);
                await _api.SendRequestAsync(_url, v4Data.GetBytes());
            });

    public async Task UnRemoveUserAsync(string userToken, string gapToken)
          => await Task.Run(async () =>
          {
              JObject json = new()
              {
                  { "action", "Unset" },
                  { "group_guid", gapToken },
                  { "member_guid", userToken },
              };
              string v4Data = await CreateDataV4Async(json.ToString(), "banGroupMember", _auth);
              await _api.SendRequestAsync(_url, v4Data.GetBytes());
          });

    public async Task NewAdminAsync(string userToken, IEnumerable<string> access, string gapToken)
            => await Task.Run(async () =>
            {
                JArray accessList = new();
                foreach (string ac in access)
                    accessList.Add(ac);

                JObject json = new()
                {
                    { "access_list", accessList },
                    { "action", "SetAdmin" },
                    { "group_guid", gapToken },
                    { "member_guid", userToken },
                };

                string v5Data = await CreateDataV5Async(json.ToString(), "setGroupAdmin", _auth);
                await _api.SendRequestAsync(_url, v5Data.GetBytes());
            });

    public async Task RemoveAdminAsync(string adminToken, string gapToken)
        => await Task.Run(async () =>
        {
            JObject json = new()
            {
                { "action", "UnsetAdmin" },
                { "group_guid", gapToken },
                { "member_guid", adminToken },
            };

            string v5Data = await CreateDataV5Async(json.ToString(), "setGroupAdmin", _auth);
            await _api.SendRequestAsync(_url, v5Data.GetBytes());
        });

    public async Task SeenCahtAsync(SeenChat seenChat)
            => await Task.Run(async () =>
            {
                JObject input = JObject.Parse("{\"seen_list\" : {\"" + seenChat.GapToken + "\":\"" + seenChat.MessageId + "\"}}");
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
}


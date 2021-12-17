namespace Rubika.Package.Bot;

/// <summary>
/// Rubika Bot Services Implement <see cref="IBot"/>
/// </summary>
public class Bot : IBot, IDisposable
{

    #region -- Depedency --

    private Action<Message> _onGetMessage;

    private string _gapToken;

    private readonly string _auth;

    private readonly IApi _api;

    private readonly List<Message> _messages;

    private readonly string _url = "https://messengerg2c63.iranlms.ir";

    public Bot(string auth)
    {
        _auth = auth;
        _auth.CreateAndSetKey();
        _messages = new();
        _api = new Api.Api();
    }

    #endregion

    public async Task CreateBotAsync(Action<Message> newMessage, string gapToken)
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
            => Task.Run(async () =>
            {
                string minId = await GetMinIdAsync(_gapToken);
                JObject json = new()
                {
                    { "limit", 10 },
                    { "min_id", minId },
                    { "object_guid", _gapToken },
                    { "sort", "FromMin" }
                };

                while (true)
                {
                    string dataReq = await CreateDataV4Async(json.ToString(), "getMessages");
                    string req = await _api.SendRequestAsync(_url, dataReq.GetBytes());
                    JObject decDataEnc = await _api.ConvertToJObjectAsync(req);
                    if (!decDataEnc.ContainsKey("err"))
                    {
                        JArray messages = JArray.Parse(decDataEnc["messages"].ToString());
                        foreach (JObject message in messages)
                            if (!_messages.Any(m => m.Id == message.SelectToken("message_id").ToString()))
                            {
                                Message newMessage = new()
                                {
                                    Id = message["message_id"].ToString(),
                                    SenderToken = message["author_object_guid"].ToString()
                                };
                                if (message.ContainsKey("text"))
                                    newMessage.Text = message["text"].ToString();
                                if (message.ContainsKey("reply_to_message_id"))
                                    newMessage.ReplyId = message["reply_to_message_id"].ToString();

                                _onGetMessage(newMessage);
                                _messages.Add(newMessage);
                            }

                        json.Remove("min_id");
                        json.Add("min_id", _messages[^1].Id);
                    }
                    Thread.Sleep(600);
                }
            }).Wait();

    public async Task<string> GetMinIdAsync(string gapToken)
        => await Task.Run(async () =>
        {
            string reqData = "{\"group_guid\":\"gToken\"}";
            reqData = reqData.Replace("gToken", gapToken);
            string strData = await CreateDataV4Async(reqData, "getGroupInfo");
            string apiCall = await _api.SendRequestAsync(_url, strData.GetBytes());
            JObject response = await _api.ConvertToJObjectAsync(apiCall);
            return response.SelectToken("chat").SelectToken("last_message").SelectToken("message_id").ToString();
        });

    public async Task<string> GetGroupTokenFromLinkAsync(string link)
        => await Task.Run(async () =>
        {
            string json = await CreateDataV4Async("{\"hash_link\":\"" + link.Replace("https://rubika.ir/joing/", "") + "\"}", "groupPreviewByJoinLink");
            string request = await _api.SendRequestAsync(_url, json.GetBytes());
            JObject response = JObject.Parse(request);
            return JObject.Parse(response["data_enc"].ToString().Crypto(true))["group"]["group_guid"].ToString();
        });

    public async Task<UserInof> GetUserInfoAsync(string userToken)
        => await Task.Run(async () =>
        {
            string createData = await CreateDataV4Async("{\"user_guid\":\"" + userToken + "\"}", "getUserInfo");
            string request = await _api.SendRequestAsync(_url, createData.GetBytes());
            string response = JObject.Parse(request)["data_enc"].ToString().Crypto(true);
            JObject resObject = JObject.Parse(response);
            JToken user = resObject["user"];
            return new UserInof()
            {
                Name = user["first_name"]?.ToString(),
                LastName = user["last_name"]?.ToString(),
                Bio = user["bio"]?.ToString(),
                UserName = user["username"]?.ToString()
            };
        });

    public async Task<string> GetGuidFromUserNameAsync(string userName)
        => await Task.Run(async () =>
        {
            string v4Data = await CreateDataV4Async("{\"username\":\"" + userName + "\"}", "getObjectByUsername");
            string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
            string response = JObject.Parse(request)["data_enc"].ToString().Crypto(true);
            JObject resJson = JObject.Parse(response);
            return resJson["user"]["user_guid"].ToString();
        });

    public async Task DeleteMessageAsync(string messageId, string gapToken)
        => await Task.Run(async () =>
        {
            string v4Data = await CreateDataV4Async("{\"message_ids\":[" + messageId + "],\"object_guid\":\"" + gapToken + "\",\"type\":\"Global\"}", "deleteMessages");
            await _api.SendRequestAsync(_url, v4Data.GetBytes());
        });

    public async Task SendMessageAsync(string text, string replyId, string gapToken)
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

                string v4Data = await CreateDataV4Async(json.ToString(), "sendMessage");
                await _api.SendRequestAsync(_url, v4Data.GetBytes());
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
                string v4Data = await CreateDataV4Async(json.ToString(), "editMessage");
                await _api.SendRequestAsync(_url, v4Data.GetBytes());
            });

    public async Task SendLocationAsync(double x, double y, string gapToken)
        => await Task.Run(async () =>
        {
            JObject json = new()
            {
                { "is_mute", false },
                { "object_guid", gapToken },
                { "rnd", new Random().Next(100000000, 999999999) },
                { "location", JObject.Parse("{\"latitude\":" + x + ",\"longitude\":" + y + "}") }
            };
            string data = await CreateDataV4Async(json.ToString(), "sendMessage");
            await _api.SendRequestAsync(_url, data.GetBytes());
        });

    public async Task<Message> GetMessageByIdAsync(string messageId, string gapToken)
            => await Task.Run(async () =>
            {
                string v4Data = await CreateDataV4Async("{\"message_ids\":[\"" + messageId + "\"],\"object_guid\":\"" + gapToken + "\"}", "getMessagesByID");
                string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
                string response = JObject.Parse(request)["data_enc"].ToString().Crypto(true);
                JObject data = JObject.Parse(response);

                JToken message = JArray.Parse(data["messages"].ToString())[0];

                return new Message()
                {
                    Id = message["message_id"].ToString(),
                    Text = message["text"]?.ToString(),
                    ReplyId = message["reply_to_message_id"]?.ToString(),
                    SenderToken = message["author_object_guid"]?.ToString()
                };
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
                string v4Data = await CreateDataV4Async(json.ToString(), "banGroupMember");
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
              string v4Data = await CreateDataV4Async(json.ToString(), "banGroupMember");
              await _api.SendRequestAsync(_url, v4Data.GetBytes());
          });

    public async Task NewAdminAsync(string userToken, string[] access, string gapToken)
            => await Task.Run(async () =>
            {
                JArray accessList = new();
                foreach (var ac in access)
                    accessList.Add(ac);

                JObject json = new()
                {
                    { "access_list", accessList },
                    { "action", "SetAdmin" },
                    { "group_guid", gapToken },
                    { "member_guid", userToken },
                };

                string v5Data = await CreateDataV5Async(json.ToString(), "setGroupAdmin");
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

            string v5Data = await CreateDataV5Async(json.ToString(), "setGroupAdmin");
            await _api.SendRequestAsync(_url, v5Data.GetBytes());
        });

    public async Task ChangeGroupTimerAsync(int time, string gapToken)
        => await Task.Run(async () =>
        {
            string v4Data = await CreateDataV4Async("{\"group_guid\":\"" + gapToken + "\",\"slow_mode\":" + time + ",\"updated_parameters\":[\"slow_mode\"]}", "editGroupInfo");
            await _api.SendRequestAsync(_url, v4Data.GetBytes());
        });

    public async Task ChangeGroupLinkAsync(string gapToken)
            => await Task.Run(async () =>
            {
                string v4Data = await CreateDataV4Async("{\"group_guid\":\"" + gapToken + "\"}", "setGroupLink");
                await _api.SendRequestAsync(_url, v4Data.GetBytes());
            });

    public async Task<string> GetGroupLinkAsync(string gapToken)
            => await Task.Run(async () =>
            {
                string v4Data = await CreateDataV4Async("{\"group_guid\":\"" + gapToken + "\"}", "getGroupLink");
                string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
                string response = JObject.Parse(request)["data_enc"].ToString().Crypto(true);
                JObject responseData = JObject.Parse(response);
                return responseData["join_linl"].ToString();
            });

    #region -- Data --

    private async Task<string> CreateDataV4Async(string data, string method)
       => await Task.Run(() =>
           new JObject
           {
               { "api_version", "4" },
               { "auth", _auth },
               { "client", JObject.Parse("{\"app_name\":\"Main\",\"app_version\":\"2.8.1\",\"lang_code\":\"fa\",\"package\":\"ir.resaneh1.iptv\",\"platform\":\"Android\"}") },
               { "data_enc", data.Crypto(false) },
               { "method", method }
           }.ToString());

    private async Task<string> CreateDataV5Async(string data, string method)
        => await Task.Run(() =>
        {
            JObject json = new()
            {
                { "client", JObject.Parse("{\"app_name\":\"Main\",\"app_version\":\"2.8.1\",\"lang_code\":\"fa\",\"package\":\"ir.resaneh1.iptv\",\"platform\":\"Android\"}") },
                { "input", JObject.Parse(data) },
                { "method", method }
            };

            string dataEnc = json.ToString().Crypto(false);

            JObject jsonData = new()
            {
                { "api_version", 5 },
                { "auth", _auth },
                { "data_enc", dataEnc }
            };
            return jsonData.ToString();
        });

    #endregion
}
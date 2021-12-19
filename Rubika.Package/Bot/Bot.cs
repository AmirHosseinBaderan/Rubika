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
}


namespace Rubika.Package.Bot;

public partial class Bot : IBot, IDisposable
{
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
}

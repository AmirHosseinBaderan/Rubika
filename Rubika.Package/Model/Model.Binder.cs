namespace Rubika.Package.Model;

internal class ModelBinder
{
    public static Message CreateMessage(string json)
    {
        JObject jObject = JObject.Parse(json);
        return new Message
        {
            Id = jObject["message_id"]?.ToString(),
            Text = jObject["text"]?.ToString(),
            ReplyId = jObject["reply_to_message_id"]?.ToString(),
            SenderToken = jObject["author_object_guid"]?.ToString()
        };
    }

    public static Chat CreateChat(string json)
         => CreateChat(JObject.Parse(json.ToString()));

    public static Chat CreateChat(JToken json)
            => CreateChat(JObject.Parse(json.ToString()));

    public static Chat CreateChat(JObject json)
        => new()
        {
            TimeStamp = int.Parse(json["time"]?.ToString() ?? "0"),
            ObjectGuid = json["object_guid"]?.ToString(),
            CountUnseen = int.Parse(json["count_unseen"]?.ToString() ?? "0"),
            IsPined = bool.Parse(json["is_pinned"]?.ToString() ?? "false"),
            IsMute = bool.Parse(json["is_mute"]?.ToString() ?? "false"),
            LastMessage = ModelBinder.CreateMessage(json["last_message"].ToString()),
            Access = JArray.Parse(json["access"].ToString()).ToList().Select(acc => acc.ToString()),
            Status = json["status"]?.ToString(),
        };

    #region -- Data --

    public static async Task<string> CreateDataV4Async(string data, string method,string auth)
       => await Task.Run(() =>
           new JObject
           {
               { "api_version", "4" },
               { "auth", auth },
               { "client", CreateClient() },
               { "data_enc", data.Crypto(false) },
               { "method", method }
           }.ToString());

    public static async Task<string> CreateDataV5Async(string data, string method,string auth)
        => await Task.Run(() =>
        {
            JObject json = new()
            {
                { "client", CreateClient() },
                { "input", JObject.Parse(data) },
                { "method", method }
            };

            string dataEnc = json.Crypto(false);

            JObject jsonData = new()
            {
                { "api_version", 5 },
                { "auth", auth },
                { "data_enc", dataEnc }
            };
            return jsonData.ToString();
        });

    private static JObject CreateClient()
        => JObject.Parse("{\"app_name\":\"Main\",\"app_version\":\"2.8.1\",\"lang_code\":\"fa\",\"package\":\"ir.resaneh1.iptv\",\"platform\":\"Web\"}");

    #endregion
}

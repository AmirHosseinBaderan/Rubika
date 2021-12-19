namespace Rubika.Package.Model;

internal class ModelBinder
{
    public static Message CreateMessage(string json)
        => CreateMessage(JObject.Parse(json));

    public static Message CreateMessage(JObject json)
         => new()
         {
             Id = json["message_id"]?.ToString(),
             Text = json["text"]?.ToString(),
             ReplyId = json["reply_to_message_id"]?.ToString(),
             SenderToken = json["author_object_guid"]?.ToString()
         };

    public static GroupPreview CreateGroupPreview(JObject json)
    {
        JToken group = json["group"];
        return new(GroupGuid: group["group_guid"]?.ToString(),
            Title: group["group_title"]?.ToString(),
            Members: int.Parse(group["count_member"]?.ToString() ?? "0"),
            SlowMode: int.Parse(group["slow_mode"]?.ToString() ?? "0"),
            Description: group["description"]?.ToString(),
            ChatHistoryVisible: bool.Parse(group["chat_history_for_new_members"]?.ToString() ?? "false"));
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

    public static async Task<string> CreateDataV4Async(string data, string method, string auth)
       => await Task.Run(() =>
           new JObject
           {
               { "api_version", "4" },
               { "auth", auth },
               { "client", CreateClient() },
               { "data_enc", data.Encrypt() },
               { "method", method }
           }.ToString());

    public static async Task<string> CreateDataV5Async(string data, string method, string auth)
        => await Task.Run(() =>
        {
            string json = new JObject()
            {
                { "client", CreateClient() },
                { "input", JObject.Parse(data) },
                { "method", method }
            }.ToString().Encrypt();

            return new JObject
            {
                { "api_version", 5 },
                { "auth", auth },
                { "data_enc", json }
            }.ToString();
        });

    private static JObject CreateClient()
        => new()
        {
            { "app_name", "Main" },
            { "app_version", "2.8.1" },
            { "lang_code", "fa" },
            { "package", "ir.resaneh1.iptv" },
            { "platform", "Web" }
        };

    #endregion
}

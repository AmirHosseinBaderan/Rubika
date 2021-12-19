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
        return CreateGroupPreview(group);
    }

    public static GroupPreview CreateGroupPreview(JToken json)
        => new(GroupGuid: json["group_guid"]?.ToString(),
            Title: json["group_title"]?.ToString(),
            Members: int.Parse(json["count_members"]?.ToString() ?? "0"),
            SlowMode: int.Parse(json["slow_mode"]?.ToString() ?? "0"),
            Description: json["description"]?.ToString(),
            ChatHistoryVisible: json["chat_history_for_new_members"]?.ToString());

    public static SendMessage CreateSendMessage(JObject json)
    {
        return !json.ContainsKey("err")
            ? (new(ActionStatus.Success, CreateChatUpdate(json["chat_update"]), CreateMessageUpdate(json["message_update"])))
            : (new(ActionStatus.Exception, null, null));
    }

    public static DeleteMessage CreateDeleteMessage(JObject json)
    {
        if (!json.ContainsKey("err"))
        {
            JArray messages = JArray.Parse(json["message_updates"].ToString());
            List<MessageUpdate> messageUpdates = new();
            foreach (JObject msu in messages)
                messageUpdates.Add(CreateMessageUpdate(msu));

            return new(ActionStatus.Success, CreateChatUpdate(json["chat_update"]), messageUpdates);
        }
        return new(ActionStatus.Exception, null, null);

    }

    public static ChatUpdate CreateChatUpdate(JToken json)
        => new()
        {
            Action = json["action"]?.ToString(),
            ObjectGuid = json["object_guid"]?.ToString(),
            Type = json["type"]?.ToString(),
            UpdateParameters = json["updated_parameters"] != null ?
                JArray.Parse(json["updated_parameters"].ToString()).ToList().Select(up => up.ToString())
                    : default,
        };

    public static MessageUpdate CreateMessageUpdate(JToken json)
        => new()
        {
            Action = json["action"]?.ToString(),
            Id = json["message_id"]?.ToString(),
            ObjectGuid = json["object_guid"]?.ToString(),
            PervId = json["perv_message_id"]?.ToString(),
            Type = json["type"]?.ToString(),
            TimeStamp = json["state"]?.ToString(),
            Message = json["message"] != null
                    ? CreateMessage(json["message"].ToString())
                    : default
        };

    public static UserInof CreateUserInfo(JToken user)
        => new()
        {
            Name = user["first_name"]?.ToString(),
            LastName = user["last_name"]?.ToString(),
            Bio = user["bio"]?.ToString(),
            UserName = user["username"]?.ToString(),
            UserGuid = user["user_guid"]?.ToString()
        };

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
            LastMessage = json["last_message"] != null ? CreateMessage(json["last_message"].ToString()) : default,
            Access = JArray.Parse(json["access"].ToString()).ToList().Select(acc => acc.ToString()),
            Status = json["status"]?.ToString(),
        };

    public static Channel CreateChannel(JToken channel)
        => new()
        {
            CahnnelGuid = channel["channel_guid"]?.ToString(),
            ShareLink = channel["share_url"]?.ToString(),
            Title = channel["channel_title"]?.ToString(),
            Type = channel["channel_type"]?.ToString(),
            UserName = channel["username"]?.ToString(),
            Members = int.Parse(channel["count_members"]?.ToString() ?? "0"),
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

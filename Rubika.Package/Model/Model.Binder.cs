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

}

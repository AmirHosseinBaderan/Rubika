namespace Rubika.Package;

public class DictToClass
{
    public DictToClass(Dictionary<string, JToken> message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        foreach (var kv in message)
        {
            if (kv.Value.Type == JTokenType.Object)
            {
                JObject jsonObject = kv.Value as JObject;
                if (jsonObject != null)
                {
                    DictToClass dictObj = new DictToClass(jsonObject.ToObject<Dictionary<string, JToken>>());
                    this.GetType().GetProperty(kv.Key)?.SetValue(this, dictObj);
                }
            }
            else if (kv.Value.Type == JTokenType.Array)
            {
                List<object> list = new List<object>();
                foreach (var item in kv.Value)
                {
                    if (item.Type == JTokenType.Object)
                    {
                        JObject jsonItem = item as JObject;
                        if (jsonItem != null)
                        {
                            DictToClass dictObj = new DictToClass(jsonItem.ToObject<Dictionary<string, JToken>>());
                            list.Add(dictObj);
                        }
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
                this.GetType().GetProperty(kv.Key)?.SetValue(this, list);
            }
            else
            {
                this.GetType().GetProperty(kv.Key)?.SetValue(this, kv.Value);
            }
        }
    }
}

public static class ClassToDict
{
    public static Dictionary<string, object> Convert(DictToClass dictToClassObject)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        var properties = dictToClassObject.GetType().GetProperties();

        foreach (var prop in properties)
        {
            var val = prop.GetValue(dictToClassObject);

            if (!(val is DictToClass) && !(val is Dictionary<string, JToken>))
            {
                result.Add(prop.Name, val);
            }
            else if (val is DictToClass)
            {
                result.Add(prop.Name, Convert((DictToClass)val));
            }
            else
            {
                result.Add(prop.Name, val);
            }
        }
        return result;
    }
}

public class Message
{
    public string Auth { get; private set; }
    public string ChatId { get; private set; }
    public Bot Bot { get; private set; }
    public DictToClass Data { get; private set; }

    public Message(string auth, Dictionary<string, JToken> message, string chatId = null, Bot bot = null)
    {
        Auth = auth ?? throw new ArgumentNullException(nameof(auth));
        ChatId = chatId ?? throw new ArgumentNullException(nameof(chatId));
        Bot = bot ?? new Bot("", Auth);

        if (message == null)
        {
            throw new ArgumentException("Message dictionary cannot be null.");
        }

        Data = new DictToClass(message);
    }

    public Dictionary<string, object> Edit(string newText, List<string> metadata = null, string parseMode = null)
    {
        return Bot.EditMessage(Data.GetType().GetProperty("message_id")?.GetValue(Data).ToString(), ChatId, newText, metadata, parseMode);
    }

    public Dictionary<string, object> Forward(string to)
    {
        return Bot.ForwardMessages(ChatId, new List<string> { Data.GetType().GetProperty("message_id")?.GetValue(Data).ToString() }, to);
    }

    public Dictionary<string, object> Resend(string to, Dictionary<string, string> kwargs = null)
    {
        return Bot.ResendMessage(ChatId, Data.GetType().GetProperty("message_id")?.GetValue(Data).ToString(), to, kwargs);
    }

    public Dictionary<string, object> GetInfo()
    {
        return Bot.GetMessagesInfo(ChatId, new List<string> { Data.GetType().GetProperty("message_id")?.GetValue(Data).ToString() })[0];
    }

    public Dictionary<string, object> GetPollStatus()
    {
        return Bot.GetPollStatus(Data.GetType().GetProperty("poll_id")?.GetValue(Data).ToString());
    }

    // Add the remaining methods here similarly...
}

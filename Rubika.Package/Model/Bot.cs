namespace Rubika.Package.Model;

#region -- Models --

public record Message
{
    public string Id { get; set; }

    public string Text { get; set; }

    public string ReplyId { get; set; }

    public string SenderToken { get; set; }
}

public record UserInof
{
    public string Name { get; set; }

    public string LastName { get; set; }

    public string Bio { get; set; }

    public string UserName { get; set; }

    public string UserGuid { get; set; }
}

public record AdminAccess
{
    public const string MemberAccess = "SetMemberAcess";

    public const string JoinLink = "SetJoinLink";

    public const string PinMessage = "PinMessage";

    public const string SetAdmin = "SetAdmin";

    public const string BanUser = "BanMember";

    public const string DeleteMessage = "DeleteGlobalAllMessages";

    public const string ChangeInfo = "ChangeInfo";
}

public record Chat
{
    public string ObjectGuid { get; set; }

    public IEnumerable<string> Access { get; set; }

    public int CountUnseen { get; set; }

    public bool IsMute { get; set; }

    public bool IsPined { get; set; }

    public Message LastMessage { get; set; }

    public string Status { get; set; }

    public int TimeStamp { get; set; }
}

public record ChatUpdate
{
    public string ObjectGuid { get; set; }

    public string Action { get; set; }

    public IEnumerable<string> UpdateParameters { get; set; }

    public string Type { get; set; }
}

public record MessageUpdate
{
    public string Id { get; set; }

    public string Action { get; set; }

    public Message Message { get; set; }

    public string PervId { get; set; }

    public string ObjectGuid { get; set; }

    public string Type { get; set; }

    public string TimeStamp { get; set; }
}

public record GroupPreview(string GroupGuid, string Title, int Members, int SlowMode, string Description, string ChatHistoryVisible);

public record SeenChat(string GapToken, string MessageId);

#endregion

#region -- Response --

public record GetMessage(ActionStatus Status, Message Message);

public record SendMessage(ActionStatus Status, ChatUpdate ChatUpdate, MessageUpdate MessageUpdate);

public record DeleteMessage(ActionStatus Status, ChatUpdate ChatUpdate, IEnumerable<MessageUpdate> MessageUpdate);

public record GetUpdatesChats(ActionStatus Status, IEnumerable<Chat> Chats);

public record GetGroupPreview(ActionStatus Status, GroupPreview Group);

public record GetGroupInfo(ActionStatus Status, Chat Chat, GroupPreview Group);

public enum ActionStatus
{
    Success,
    Exception
}

#endregion
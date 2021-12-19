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

public record GroupPreview(string GroupGuid, string Title, int Members, int SlowMode, string Description, bool ChatHistoryVisible);

public record SeenChat(string GapToken, string MessageId);

#endregion

#region -- Response --

public record GetMessage(ActionStatus Status, Message Message);

public record GetUpdatesChats(ActionStatus Status, IEnumerable<Chat> Chats);

public record GetGroupPreview(ActionStatus Status, GroupPreview Group);

public enum ActionStatus
{
    Success,
    Exception
}

#endregion
namespace Rubika.Package.Bot;

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
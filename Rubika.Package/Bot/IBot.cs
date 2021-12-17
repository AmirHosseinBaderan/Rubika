namespace Rubika.Package.Bot;

/// <summary>
/// Rubika Bot Services Interface
/// </summary>
public interface IBot
{
    Task CreateBotAsync(Action<Message> message, string gapToken);

    Task<string> GetMinIdAsync(string gapToken);

    Task<string> GetGroupTokenFromLinkAsync(string link);

    Task<UserInof> GetUserInfoAsync(string userToken);

    Task<string> GetGuidFromUserNameAsync(string userName);

    Task DeleteMessageAsync(string messageId, string gapToken);

    Task SendMessageAsync(string text, string replyId, string gapToken);

    Task EditMessageAsync(string text, string messageId, string gapToken);

    Task SendLocationAsync(double x, double y, string gapToken);

    Task<Message> GetMessageByIdAsync(string messageId, string gapToken);

    Task RemoveUserAsync(string userToken, string gapToken);

    Task UnRemoveUserAsync(string userToken, string gapToken);

    Task NewAdminAsync(string userToken, IEnumerable<string> access, string gapToken);

    Task RemoveAdminAsync(string adminToken, string gapToken);

    Task ChangeGroupTimerAsync(int time, string gapToken);

    Task ChangeGroupLinkAsync(string gapToken);

    Task<string> GetGroupLinkAsync(string gapToken);

    void GetMessage();
}
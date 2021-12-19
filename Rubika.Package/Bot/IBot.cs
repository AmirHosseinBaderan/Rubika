using Rubika.Package.Model;

namespace Rubika.Package.Bot;

/// <summary>
/// Rubika Bot Services Interface
/// </summary>
public interface IBot
{
    /// <summary>
    /// Create Bot And Get New Messages Real-Time
    /// </summary>
    /// <param name="message">New Message Call Back</param>
    /// <param name="gapToken">Gap Token</param>
    /// <returns><see cref="Package.Bot.GetMessage"/></returns>
    Task CreateBotAsync(Action<GetMessage> message, string gapToken);

    Task<string> GetMinIdAsync(string gapToken);

    Task<string> GetGroupTokenFromLinkAsync(string link);

    Task<UserInof> GetUserInfoAsync(string userToken);

    Task<string> GetGuidFromUserNameAsync(string userName);

    Task DeleteMessageAsync(string messageId, string gapToken);

    Task SendMessageAsync(string text, string replyId, string gapToken);

    Task EditMessageAsync(string text, string messageId, string gapToken);

    Task SendLocationAsync(double lat, double lon, string gapToken);

    Task<Message> GetMessageByIdAsync(string messageId, string gapToken);

    Task RemoveUserAsync(string userToken, string gapToken);

    Task UnRemoveUserAsync(string userToken, string gapToken);

    Task NewAdminAsync(string userToken, IEnumerable<string> access, string gapToken);

    Task RemoveAdminAsync(string adminToken, string gapToken);

    Task ChangeGroupTimerAsync(int time, string gapToken);

    Task ChangeGroupLinkAsync(string gapToken);

    Task SeenCahtAsync(SeenChat seenChat);

    Task<GetUpdatesChats> GetChatsUpdatesAsync(string timeStamp);

    Task<string> GetGroupLinkAsync(string gapToken);

    void GetMessage();

    #region -- Group --

    Task<GetGroupPreview> GetGroupPreviewByLinkAsync(string link);

    Task<GetGroupInfo> GetGroupInfoFromTokenAsync(string gapToken);

    #endregion
}
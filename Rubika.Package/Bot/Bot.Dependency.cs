namespace Rubika.Package.Bot;

public partial class Bot : IBot, IDisposable
{

    private Action<GetMessage> _onGetMessage;

    private string _gapToken;

    private readonly string _auth;

    private readonly IApi _api;

    private readonly List<Message> _messages;

    private readonly string _url = "https://messengerg2c63.iranlms.ir";

    public Bot(string auth)
    {
        _auth = auth;
        _auth.CreateAndSetKey();
        _messages = new();
        _api = new Api.Api();
    }
}

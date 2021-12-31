using Microsoft.AspNetCore.TestHost;
using System.Net.WebSockets;

namespace Rubika.Package.Bot;

public partial class Bot : IBot, IDisposable
{

    private Action<GetMessage> _onGetMessage;

    private string _gapToken;

    private readonly string _auth;

    private readonly IApi _api;

    private readonly List<Message> _messages;

    private readonly string _url = "https://messengerg2c63.iranlms.ir";

    private readonly string _wsUrl = "wss://jsocket5.iranlms.ir:80";

    private readonly ClientWebSocket _clientWS;

    private readonly CancellationTokenSource _cancellationTokenSource;

    public Bot(string auth)
    {
        _clientWS = new();
        _cancellationTokenSource = new();
        _auth = auth;
        _auth.CreateAndSetKey();
        _messages = new();
        _api = new Api.Api();
    }
}

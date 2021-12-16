namespace Rubika.Package.Api;

internal interface IApi
{
    Task<string> SendRequestAsync(string url, byte[] data);
}

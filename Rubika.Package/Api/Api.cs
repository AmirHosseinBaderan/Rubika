using System;

namespace Rubika.Package.Api;

internal class Api : IApi, IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public async Task<string> SendRequestAsync(string url, byte[] data)
            => await Task.Run(async () =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json; charset=UTF-8";
                    request.ContentLength = data.Length;
                    await request.GetRequestStream().WriteAsync(data);
                    Stream responseStream = request.GetResponse().GetResponseStream();
                    using StreamReader sr = new(responseStream);
                    return await sr.ReadToEndAsync();
                }
                catch
                {
                    return "";
                }
            });
}
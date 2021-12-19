namespace Rubika.Package.Api;

internal class Api : IApi, IDisposable
{
    public async Task<JObject> ConvertToJObjectAsync(string response)
            => await Task.Run(() =>
            {
                if (response != "")
                {
                    JObject resObject = JObject.Parse(response);
                    string encData = resObject["data_enc"].ToString().Decrypt();
                    return JObject.Parse(encData);
                }
                return new JObject()
                {
                    {"err",true }
                };
            });

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
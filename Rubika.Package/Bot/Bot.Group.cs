namespace Rubika.Package.Bot;

public partial class Bot : IBot, IDisposable
{
    public async Task<GetGroupPreview> GetGroupPreviewByLinkAsync(string link)
         => await Task.Run(async () =>
         {
             string json = await CreateDataV4Async("{\"hash_link\":\"" + link.Replace("https://rubika.ir/joing/", "") + "\"}", "groupPreviewByJoinLink", _auth);
             string request = await _api.SendRequestAsync(_url, json.GetBytes());
             JObject response = JObject.Parse(request);
             if (!response.ContainsKey("err"))
             {
                 JObject group = JObject.Parse(response["data_enc"].ToString().Decrypt());
                 GroupPreview groupPreview = CreateGroupPreview(group);
                 return new GetGroupPreview(ActionStatus.Success, groupPreview);
             }
             return new GetGroupPreview(ActionStatus.Exception, null);
         });

    public async Task<string> GetGroupLinkAsync(string gapToken)
           => await Task.Run(async () =>
           {
               string v4Data = await CreateDataV4Async("{\"group_guid\":\"" + gapToken + "\"}", "getGroupLink", _auth);
               string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
               string response = JObject.Parse(request)["data_enc"].ToString().Decrypt();
               JObject responseData = JObject.Parse(response);
               return responseData["join_link"].ToString();
           });

    public async Task ChangeGroupLinkAsync(string gapToken)
           => await Task.Run(async () =>
           {
               string v4Data = await CreateDataV4Async("{\"group_guid\":\"" + gapToken + "\"}", "setGroupLink", _auth);
               await _api.SendRequestAsync(_url, v4Data.GetBytes());
           });

    public async Task ChangeGroupTimerAsync(int time, string gapToken)
            => await Task.Run(async () =>
            {
                string v4Data = await CreateDataV4Async("{\"group_guid\":\"" + gapToken + "\",\"slow_mode\":" + time + ",\"updated_parameters\":[\"slow_mode\"]}", "editGroupInfo", _auth);
                await _api.SendRequestAsync(_url, v4Data.GetBytes());
            });

    public async Task<string> GetGroupTokenFromLinkAsync(string link)
       => await Task.Run(async () =>
       {
           GetGroupPreview group = await GetGroupPreviewByLinkAsync(link);
           return group.Status == ActionStatus.Success ?
                   group.Group.GroupGuid : "";
       });
}
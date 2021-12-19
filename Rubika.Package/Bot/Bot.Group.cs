namespace Rubika.Package.Bot;

public partial class Bot : IBot, IDisposable
{
    public async Task<GetGroupPreview> GetGroupPreviewByLinkAsync(string link)
         => await Task.Run(async () =>
         {
             string json = await CreateDataV4Async("{\"hash_link\":\"" + link.Replace("https://rubika.ir/joing/", "") + "\"}", "groupPreviewByJoinLink", _auth);
             string request = await _api.SendRequestAsync(_url, json.GetBytes());
             JObject response = await _api.ConvertToJObjectAsync(request);
             if (!response.ContainsKey("err"))
             {
                 GroupPreview groupPreview = CreateGroupPreview(response["group"]);
                 return new GetGroupPreview(ActionStatus.Success, groupPreview);
             }
             return new GetGroupPreview(ActionStatus.Exception, null);
         });

    public async Task<string> GetGroupLinkAsync(string gapToken)
           => await Task.Run(async () =>
           {
               if (gapToken == null) gapToken = _gapToken;

               string v4Data = await CreateDataV4Async("{\"group_guid\":\"" + gapToken + "\"}", "getGroupLink", _auth);
               string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
               JObject responseData = await _api.ConvertToJObjectAsync(request);
               return responseData["join_link"].ToString();
           });

    public async Task ChangeGroupLinkAsync(string gapToken)
           => await Task.Run(async () =>
           {
               if (gapToken == null) gapToken = _gapToken;

               string v4Data = await CreateDataV4Async("{\"group_guid\":\"" + gapToken + "\"}", "setGroupLink", _auth);
               await _api.SendRequestAsync(_url, v4Data.GetBytes());
           });

    public async Task ChangeGroupTimerAsync(int time, string gapToken)
            => await Task.Run(async () =>
            {
                if (gapToken == null) gapToken = _gapToken;

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

    public async Task<GetGroupInfo> GetGroupInfoFromTokenAsync(string gapToken)
            => await Task.Run(async () =>
            {
                if (gapToken == null) gapToken = _gapToken;

                string reqData = "{\"group_guid\":\"gToken\"}";
                reqData = reqData.Replace("gToken", gapToken);
                string strData = await CreateDataV4Async(reqData, "getGroupInfo", _auth);
                string apiCall = await _api.SendRequestAsync(_url, strData.GetBytes());
                JObject response = await _api.ConvertToJObjectAsync(apiCall);
                if (!response.ContainsKey("err"))
                {
                    Chat chat = CreateChat(response["chat"]);
                    GroupPreview group = CreateGroupPreview(response["group"]);
                    return new GetGroupInfo(ActionStatus.Success, chat, group);
                }
                return new GetGroupInfo(ActionStatus.Exception, null, null);
            });
}
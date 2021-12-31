namespace Rubika.Package.Bot;

public partial class Bot : IBot, IDisposable
{
    public async Task<UserInof> GetUserInfoAsync(string userToken)
   => await Task.Run(async () =>
   {
       string createData = await CreateDataV4Async("{\"user_guid\":\"" + userToken + "\"}", "getUserInfo", _auth);
       string request = await _api.SendRequestAsync(_url, createData.GetBytes());
       JObject resObject = await _api.ConvertToJObjectAsync(request);
       JToken user = resObject["user"];
       return CreateUserInfo(user);
   });

    public async Task<string> GetGuidFromUserNameAsync(string userName)
        => await Task.Run(async () =>
        {
            string v4Data = await CreateDataV4Async("{\"username\":\"" + userName + "\"}", "getObjectByUsername", _auth);
            string request = await _api.SendRequestAsync(_url, v4Data.GetBytes());
            JObject resJson = await _api.ConvertToJObjectAsync(request);
            return !resJson.ContainsKey("err") ?
                resJson["user"]["user_guid"].ToString() : "";
        });

    public async Task RemoveUserAsync(string userToken, string gapToken)
        => await Task.Run(async () =>
        {
            if (gapToken == null) gapToken = _gapToken;

            JObject json = new()
            {
                { "action", "Set" },
                { "group_guid", gapToken },
                { "member_guid", userToken },
            };
            string v4Data = await CreateDataV4Async(json.ToString(), "banGroupMember", _auth);
            await _api.SendRequestAsync(_url, v4Data.GetBytes());
        });

    public async Task UnRemoveUserAsync(string userToken, string gapToken)
          => await Task.Run(async () =>
          {
              if (gapToken == null) gapToken = _gapToken;

              JObject json = new()
              {
                  { "action", "Unset" },
                  { "group_guid", gapToken },
                  { "member_guid", userToken },
              };
              string v4Data = await CreateDataV4Async(json.ToString(), "banGroupMember", _auth);
              await _api.SendRequestAsync(_url, v4Data.GetBytes());
          });

    public async Task NewAdminAsync(string userToken, IEnumerable<string> access, string gapToken)
            => await Task.Run(async () =>
            {
                if (gapToken == null) gapToken = _gapToken;

                JArray accessList = new();
                foreach (string ac in access)
                    accessList.Add(ac);

                JObject json = new()
                {
                    { "access_list", accessList },
                    { "action", "SetAdmin" },
                    { "group_guid", gapToken },
                    { "member_guid", userToken },
                };

                string v5Data = await CreateDataV5Async(json.ToString(), "setGroupAdmin", _auth);
                await _api.SendRequestAsync(_url, v5Data.GetBytes());
            });

    public async Task RemoveAdminAsync(string adminToken, string gapToken)
        => await Task.Run(async () =>
        {
            if (gapToken == null) gapToken = _gapToken;

            JObject json = new()
            {
                { "action", "UnsetAdmin" },
                { "group_guid", gapToken },
                { "member_guid", adminToken },
            };

            string v5Data = await CreateDataV5Async(json.ToString(), "setGroupAdmin", _auth);
            await _api.SendRequestAsync(_url, v5Data.GetBytes());
        });

    public async Task<GetUserByUserName> GetUserByUserNameAsync(string userName)
        => await Task.Run(async () =>
        {
            JObject getObject = await GetObjectByUserNameAsync(userName);
            if (!getObject.ContainsKey("err"))
            {
                UserInof user = CreateUserInfo(getObject["user"]);
                return new GetUserByUserName(user)
                {
                    Exist = bool.Parse(getObject["exist"]?.ToString() ?? "false"),
                    Status = ActionStatus.Success,
                    Type = getObject["type"]?.ToString(),
                    Chat = CreateChat(getObject["chat"])
                };
            }
            return new GetUserByUserName(null) { Status = ActionStatus.Exception };
        });
}
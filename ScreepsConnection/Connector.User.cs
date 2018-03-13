using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScreepsConnection
{
    public partial class Connector
    {
		public async Task<UserInfo> GetMyUserInfo()
		{ 
			var node = await RequestGet($"/api/auth/me");
			if (node["ok"] == 1){
				var user = new UserInfo(node["username"], node["gcl"], null);
				return user;
			}
			return null;
		}
    }
}

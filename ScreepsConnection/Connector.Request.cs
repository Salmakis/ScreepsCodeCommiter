using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScreepsConnection
{
	public partial class Connector
	{
		public async Task<JsonValue> RequestGet(string request)
		{
			if (request.Contains("?")){
				request += $"&_token={token}";
			}else{
				request += $"?_token={token}";
			}
			try
			{
				var jsonString = await client.GetStringAsync($"{baseAdress}{request}");
				return Parse(jsonString);
			}
			catch (Exception e)
			{
				error = new ConnectorError(e.Message);
				return Parse("{\"ok\":0}");
			}
		}

		public JsonValue Parse(string jsonString)
		{
			try
			{
				var jsonValue = JsonValue.Parse(jsonString);
				return jsonValue;
			}
			catch (Exception e)
			{
				this.error = new ConnectorError($"json parse error: {e.Message}");
				//return a not ok signal json
				return JsonValue.Parse("{\"ok\":0}");
			}
		}
	}
}

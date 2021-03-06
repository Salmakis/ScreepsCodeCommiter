﻿/*Copyright (c) 2018 Annika Ryll License: MIT

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ScreepsConnection
{
	public partial class Connector
	{

		public async Task<JsonValue> RequestPost(string uriPart,string content)
		{
			if (uriPart.Contains("?"))
			{
				uriPart += $"&_token={token}";
			}
			else
			{
				uriPart += $"?_token={token}";
			}
			try
			{
				Console.WriteLine($"posting to {uriPart} with {content}");
				//var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseAdress}{uriPart}");
				var stringContent = new StringContent(content,Encoding.UTF8, "application/json");
				var response = await client.PostAsync($"{baseAdress}{uriPart}",stringContent);
				return Parse(await response.Content.ReadAsStringAsync());
			}
			catch (Exception e)
			{
				error = new ConnectorError(e.Message);
				return Parse("{\"ok\":0}");
			}
		}

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

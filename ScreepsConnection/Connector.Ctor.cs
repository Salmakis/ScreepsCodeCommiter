/*Copyright (c) 2018 Annika Ryll License: MIT

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
using System.Net.Http;
using System.Threading.Tasks;
using System.Json;
using ScreepsConnection;
//05e59336-8feb-433c-b553-b97d260a5780
namespace ScreepsConnection
{
	public partial class Connector
	{
		string token;
		private static HttpClient client = new HttpClient();
		string baseAdress;

		public string Token => token;
		public string Adress => baseAdress;

		private string AddToken()
		{
			return $"&_token={token}";
		}

		public Connector(string token, string server = "http://screeps.com/")
		{
			//fallback for screepsmod-auth
			try
			{
				baseAdress = server;
				//client.BaseAddress = new Uri(server);
			}
			catch (Exception e)
			{
				error = new ConnectorError("invalid server address");
			}
			this.token = token;
		}
	}
}

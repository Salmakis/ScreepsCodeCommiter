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
			//client.DefaultRequestHeaders.Add("X-Token",token);
			//client.DefaultRequestHeaders.Add("X-Username", token);

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

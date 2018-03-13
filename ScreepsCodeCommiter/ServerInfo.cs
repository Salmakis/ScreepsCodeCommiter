using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreepsCodeCommiter
{
	public class ServerInfo
	{
		public static List<ServerInfo> servers = new List<ServerInfo>();

		public string Address { get; set; }
		public string Token { get; set; }

		public ServerInfo(string add, string token)
		{
			Address = add;
			Token = token;
		}

		public ServerInfo()
		{

		}

		public override string ToString()
		{
			return Address;
		}
	}
}
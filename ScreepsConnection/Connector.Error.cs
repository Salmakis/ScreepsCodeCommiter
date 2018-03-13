using System;
using System.Collections.Generic;
using System.Text;

namespace ScreepsConnection
{
	public class ConnectorError
	{
		public string Message{ get; }

		public ConnectorError(string msg)
		{
			Message = msg;
		}


		public override string ToString()
		{
			return Message;
		}
	}

	public partial class Connector
	{
		ConnectorError error;

		public bool HasError => error != null;

		public ConnectorError GetLastError()
		{
			if (null != error){
				var ret = error;
				error = null;
				return ret;
			}
			return new ConnectorError("no error");
		}

	}
}

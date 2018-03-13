using System;
using System.Collections.Generic;
using System.Text;

namespace ScreepsConnection
{
	public class UserInfo
	{
		public string Name { get; }
		public int Gcl { get; }
		public UserBadge Badge { get; }

		public UserInfo(string name, int gcl, UserBadge badge = null)
		{
			this.Name = name;
			this.Gcl = gcl;
			this.Badge = badge;
		}
	}
}

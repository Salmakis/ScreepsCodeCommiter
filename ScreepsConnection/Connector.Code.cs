using ScreepsConnection;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScreepsConnection
{
	public partial class Connector
	{
		public async Task<IEnumerable<ScreepsCodeFile>> GetCode(string branch = "default")
		{
			var node = await RequestGet($"/api/user/code?branch={branch}");
			var codeList = new List<ScreepsCodeFile>();
			if (node["ok"] == 1)
			{
				foreach (var subNode in node["modules"])
				{
					KeyValuePair<string, JsonValue> nodePair = (KeyValuePair<string, JsonValue>)subNode;
					var newFile = new ScreepsCodeFile(nodePair.Key, nodePair.Value.ToString());
					codeList.Add(newFile);
				}
			}
			return codeList;
		}

		/// <summary>
		/// get the branches, the current active one (world) will be the first one in the list
		/// </summary>
		public async Task<IEnumerable<string>> GetBranches()
		{
			var list = new LinkedList<string>();
			
			var node = (await RequestGet($"/api/user/branches"));
			if(node["ok"] == 1){
				foreach (var subNode in ((JsonArray)node["list"]))
				{
					if (subNode.ContainsKey("activeWorld") && subNode["activeWorld"] == true){
						list.AddFirst(subNode["branch"]);
					}
					else{
						list.AddLast(subNode["branch"]);
					}
				}
				return list;
			}	
			return null;
		}
	}
}

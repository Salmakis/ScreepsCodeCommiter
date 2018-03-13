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
		public async void UploadCode(string branch,IEnumerable<ScreepsCodeFile> codeFiles)
		{
			var json = new JsonObject();
			//add the branch
			json.Add("branch", "default");
			var modulesNode = new JsonObject();
			foreach (var codeFile in codeFiles)
			{
				modulesNode.Add(codeFile.FileName, codeFile.FileContent);
			}
		}

		public async Task<IEnumerable<ScreepsCodeFile>> GetCode(string branch = "default")
		{
			var node = await RequestGet($"/api/user/code?branch={branch}");
			var codeList = new List<ScreepsCodeFile>();
			if (node["ok"] == 1)
			{
				foreach (var subNode in node["modules"])
				{
					KeyValuePair<string, JsonValue> nodePair = (KeyValuePair<string, JsonValue>)subNode;
					var newFile = new ScreepsCodeFile(nodePair.Key, nodePair.Value);
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

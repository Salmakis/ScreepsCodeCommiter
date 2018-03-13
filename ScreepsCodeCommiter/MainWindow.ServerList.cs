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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ScreepsCodeCommiter
{

	/// <summary>
	/// This part contains all methods related for the managment of the server list
	/// </summary>
	public partial class MainWindow
	{
		/// <summary>
		/// perfrom a small test on the given server (by downloading the user info)
		/// </summary>
		public async void ButtonTestServer(object sender, RoutedEventArgs e)
		{
			if (serverList.SelectedItem != null)
			{
				var server = (ServerInfo)serverList.SelectedItem;

				var conn = new Connector(server.Token, server.Address);
				if (conn.HasError)
				{
					LogActivity("could not create connector - error:" + conn.GetLastError(), true);
					return;
				}
				var userInfo = await conn.GetMyUserInfo();
				if (userInfo == null)
				{
					LogActivity("Error with server " + server.Address, true);
					LogActivity(conn.GetLastError() + server.Address, true);
					return;
				}
				else
				{
					LogActivity($"hello {userInfo.Name}, this server works");

				}
			}
			else
			{
				MessageBox.Show("select a server first");
				return;
			}
		}

		/// <summary>
		/// Load the list of servers from the users config folder
		/// </summary>
		private void LoadServerList()
		{
			var path = GetConfigSaveFolder();
			using (StreamReader inputFile = new StreamReader(path + @"serverList"))
			{
				while (!inputFile.EndOfStream)
				{
					var split = inputFile.ReadLine().Split('|');
					if (split.Length == 2)
					{
						ServerInfo.servers.Add(new ServerInfo(split[0], split[1]));
					}
				}
			}
		}

		/// <summary>
		/// save the list of servers to the users config folder
		/// </summary>
		private void SaveServerList()
		{
			var path = GetConfigSaveFolder();
			var lines = ServerInfo.servers.Select(x => { return $"{ x.Address}|{x.Token}"; });

			using (StreamWriter outputFile = new StreamWriter(path + @"serverList"))
			{
				foreach (string line in lines)
					outputFile.WriteLine(line);
			}
		}

		/// <summary>
		/// button to add a new server
		/// </summary>
		public void ButtonAddServer(object sender, RoutedEventArgs e)
		{
			var srvWindow = new ServerWindow();
			srvWindow.SetInfo(new ServerInfo());
			srvWindow.ShowDialog();
			RefreshServerList();
			SaveServerList();
		}

		/// <summary>
		/// button to edit an existin server (currently selcted) server
		/// </summary>
		public void ButtonEditServer(object sender, RoutedEventArgs e)
		{
			var srvWindow = new ServerWindow();
			srvWindow.SetInfo((ServerInfo)serverList.SelectedItem);
			srvWindow.ShowDialog();
			RefreshServerList();
			SaveServerList();
		}

		/// <summary>
		/// button to remove a given (currently selcted) server
		/// </summary>
		public void ButtonRemoveServer(object sender, RoutedEventArgs e)
		{
			if (ServerInfo.servers.Contains(serverList.SelectedItem))
			{
				ServerInfo.servers.Remove((ServerInfo)serverList.SelectedItem);
			}
			RefreshServerList();
			SaveServerList();
		}

		/// <summary>
		/// update the label texts if the user selects a server
		/// </summary>
		public void ServerSelected(object sender, RoutedEventArgs e)
		{
			if (serverList.SelectedItem != null)
			{
				lblServerAdd.Content = ((ServerInfo)serverList.SelectedItem).Address;
				lblServerToken.Content = ((ServerInfo)serverList.SelectedItem).Token;
			}
			else
			{
				lblServerAdd.Content = "";
				lblServerToken.Content = "";
			}
		}

		/// <summary>
		/// refresh the combobox for server list
		/// </summary>
		private void RefreshServerList()
		{
			ServerInfo prevSelected = (ServerInfo)serverList.SelectedItem;
			serverList.Items.Clear();
			foreach (var server in ServerInfo.servers)
			{
				serverList.Items.Add(server);
			}
			if (ServerInfo.servers.Contains(prevSelected))
			{
				serverList.SelectedItem = prevSelected;
			}
			else if (serverList.SelectedItem == null && serverList.Items.Count > 0)
			{
				serverList.SelectedItem = serverList.Items.GetItemAt(serverList.Items.Count - 1);
			}

			if (serverList.SelectedItem == null)
			{
				((Button)FindName("btnEdit")).IsEnabled = false;
				((Button)FindName("btnRemove")).IsEnabled = false;
				((Button)FindName("btnTest")).IsEnabled = false;
				lblServerAdd.Content = "";
				lblServerToken.Content = "";
			}
			else
			{
				((Button)FindName("btnEdit")).IsEnabled = true;
				((Button)FindName("btnRemove")).IsEnabled = true;
				((Button)FindName("btnTest")).IsEnabled = true;
			}
		}
	}
}

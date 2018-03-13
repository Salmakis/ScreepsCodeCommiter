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
	/// This part contains all methods related for the download function
	/// </summary>
	public partial class MainWindow
	{
		/// <summary>
		/// update the infos in the download tab (such as branches, username etc)
		/// </summary>
		/// <param name="serverInfo"></param>
		private async void UpdateDownloadTab(ServerInfo serverInfo)
		{
			((Grid)FindName("downGrid")).IsEnabled = false;
			if (null == currentConnector ||
				currentConnector.Adress != serverInfo.Address ||
				currentConnector.Token != serverInfo.Token)
			{
				currentConnector = new Connector(serverInfo.Token, serverInfo.Address);
			}
			LogActivity($"getting user info from {serverInfo.Address}");
			var userInfo = await currentConnector.GetMyUserInfo();
			if (null != userInfo)
			{
				LogActivity($"found user {userInfo.Name}");
				((Label)FindName("downStatus")).Content = $"Hello {userInfo.Name} checking your Branches...";
			}
			else
			{
				LogActivity($"could not fetch user data, token?", true);
				((Label)FindName("downStatus")).Content = $"could not fetch user data, token?";
				return;
			}

			ComboBox bListDown = (ComboBox)FindName("cmbDownBranches");

			RefreshBranchLists(bListDown);
			((Label)FindName("downStatus")).Content = $"Hello {userInfo.Name} here can you download your code.";
			((Grid)FindName("downGrid")).IsEnabled = true;
		}

		/// <summary>
		/// check to be sure overwriting is ok
		/// </summary>
		private void CheckOverwriteDownload(object sender, RoutedEventArgs e)
		{
			var cBox = ((CheckBox)FindName("chkSaveOverwrite"));
			var btn = ((Button)FindName("btnStartDownload"));
			if (cBox?.IsChecked == true)
			{
				btn.IsEnabled = true;
			}else{
				btn.IsEnabled = false;
			}
		}

		/// <summary>
		/// start the download
		/// </summary>
		private async void ButtonStartDownload(object sender, RoutedEventArgs e)
		{
			if (((CheckBox)FindName("chkSaveOverwrite"))?.IsChecked != true)
			{
				return;
			}

			ComboBox bListDown = (ComboBox)FindName("cmbDownBranches");
			string branchName = (string)bListDown.SelectedItem;
			string folder = ((TextBox)FindName("downloadPathBox")).Text;
			bool createBranchFolder = ((CheckBox)FindName("chkCreateSub")).IsChecked == true;

			if (createBranchFolder)
			{
				folder += $"/{branchName}/";
			}
			else
			{
				folder += "/";
			}

			LogActivity($"Downloading code of branch {branchName} from {currentConnector.Adress}", false);
			var codeList = await currentConnector.GetCode(branchName);
			if (codeList != null && codeList.Count() > 0)
			{
				LogActivity($"recieved {codeList.Count()} code modules/files");

				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}

				foreach (var item in codeList)
				{
					LogActivity($"saving file {item.FileName}.js ...");
					using (StreamWriter outputFile = new StreamWriter($"{folder}{item.FileName}.js"))
					{
						outputFile.Write(item.FileContent);
					}
				}
			}
			else
			{
				LogActivity($"no code files downloaded");
			}
		}
	}
}

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
using System.Windows;
using System.Windows.Controls;

namespace ScreepsCodeCommiter
{
	/// <summary>
	/// This part contains all methods related for the code upload function
	/// </summary>
	public partial class MainWindow
	{
		/// <summary>
		/// update the infos in the upload tab (such as branches, username etc)
		/// </summary>
		/// <param name="serverInfo"></param>
		private async void UpdateUploadTab(ServerInfo serverInfo)
		{
			((Grid)FindName("upGrid")).IsEnabled = false;
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
				((Label)FindName("upStatus")).Content = $"Hello {userInfo.Name} checking your Branches...";
			}
			else
			{
				LogActivity($"could not fetch user data, token?", true);
				((Label)FindName("upStatus")).Content = $"could not fetch user data, token?";
				return;
			}

			ComboBox bListDown = (ComboBox)FindName("cmbUpBranches");

			RefreshBranchLists(bListDown);
			((Label)FindName("upStatus")).Content = $"Hello {userInfo.Name} here can you upload your code to a branch";
			((Grid)FindName("upGrid")).IsEnabled = true;
		}

		/// <summary>
		/// on decision for existing branch
		/// </summary>
		private void RadioBranchSelectExisting(object sender, RoutedEventArgs e)
		{
			
		}
	}
}

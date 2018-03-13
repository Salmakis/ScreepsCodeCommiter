using ScreepsConnection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

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
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ComboBox serverList;
		Label lblServerAdd;
		Label lblServerToken;
		RichTextBox logBox;

		Connector currentConnector;

		public MainWindow()
		{
			InitializeComponent();
			serverList = (ComboBox)FindName("listServers");
			lblServerAdd = (Label)FindName("lblAddress");
			lblServerToken = (Label)FindName("lblToken");
			logBox = (RichTextBox)FindName("logger");
			LoadServerList();
			RefreshServerList();
		}

		private void DoNothing(object sender, RoutedEventArgs e)
		{
			//just to prevent the stupid WPF "routing feature"
			e.Handled = true;
		}

		private string GetSaveFolder()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			System.IO.Directory.CreateDirectory(path + @"\ScreepsCodeCommiter\");
			return path + @"\ScreepsCodeCommiter\";
		}

		public async void TestServer(object sender, RoutedEventArgs e)
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

		private void LoadServerList()
		{
			var path = GetSaveFolder();
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

		private void SaveServerList()
		{
			var path = GetSaveFolder();
			var lines = ServerInfo.servers.Select(x => { return $"{ x.Address}|{x.Token}"; });

			using (StreamWriter outputFile = new StreamWriter(path + @"serverList"))
			{
				foreach (string line in lines)
					outputFile.WriteLine(line);
			}
		}

		private void ButtonAddServer(object sender, RoutedEventArgs e)
		{
			var srvWindow = new ServerWindow();
			srvWindow.SetInfo(new ServerInfo());
			srvWindow.ShowDialog();
			RefreshServerList();
			SaveServerList();
		}

		private void ButtonEditServer(object sender, RoutedEventArgs e)
		{
			var srvWindow = new ServerWindow();
			srvWindow.SetInfo((ServerInfo)serverList.SelectedItem);
			srvWindow.ShowDialog();
			RefreshServerList();
			SaveServerList();
		}

		private void ButtonRemoveServer(object sender, RoutedEventArgs e)
		{
			if (ServerInfo.servers.Contains(serverList.SelectedItem))
			{
				ServerInfo.servers.Remove((ServerInfo)serverList.SelectedItem);
			}
			RefreshServerList();
			SaveServerList();
		}

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

		public void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
		{	
			var ctrl = (TabControl)sender;
			if (null != ctrl.SelectedItem)
			{
				var tab = (TabItem)ctrl?.SelectedItem;
				switch (tab?.Name)
				{
					case "tabDownload":
						if (null != serverList.SelectedItem)
						{
							ServerInfo serverInfo = (ServerInfo)serverList.SelectedItem;
							((Label)FindName("downStatus")).Content = $"Please wait, getting info from {serverInfo.Address}";
							UpdateDownloadTab(serverInfo);
						}
						else
						{
							((Label)FindName("downStatus")).Content = $"Please create and Select a server in settings tab.";
						}
						break;
					case "tabUpload":
						((Grid)FindName("downGrid")).IsEnabled = false;
						break;
				}
			}
		}

		public void ClearLog(object sender, RoutedEventArgs e)
		{
			logBox.Document.Blocks.Clear();
		}

		public void LogActivity(string text, bool error = false)
		{
			Paragraph p = new Paragraph(new Run(text));
			p.LineHeight = 5;
			if (error)
			{
				p.Foreground = Brushes.Red;
			}
			logBox.Document.Blocks.Add(p);
			logBox.ScrollToEnd();
		}

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

			RefreshBranchLists();
			((Label)FindName("downStatus")).Content = $"Hello {userInfo.Name} here can you download your code.";
			((Grid)FindName("downGrid")).IsEnabled = true;
		}

		private async void RefreshBranchLists()
		{
			var branches = await GetBranches();
			if (null != branches)
			{
				ComboBox bListDown = (ComboBox)FindName("cmbDownBranches");
				var prefSelection = bListDown.SelectedItem;
				bListDown.Items.Clear();
				foreach (string branch in branches)
				{
					bListDown.Items.Add(branch);
				}
				
				if (null != prefSelection){
					bListDown.SelectedItem = prefSelection;
				}
				else{
					bListDown.SelectedItem = branches.First();
				}
				((Label)FindName("dwnActiveServer")).Content = branches.First();

			}
			else
			{
				return;
			}
		}

		private async Task<IEnumerable<string>> GetBranches()
		{
			LogActivity($"Downloading Branch List...", false);
			IEnumerable<string> branches = await currentConnector.GetBranches();
			if (null == branches)
			{
				LogActivity($"Could not download Branch List", true);
			}else{
				LogActivity($"found {branches.Count()} branch names");
			}
			return branches;
		}

		private void RefreshBranchButton(object sender, RoutedEventArgs e)
		{
			RefreshBranchLists();
		}
		private void PickDownlodTargetFolder(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{	
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK){
					((TextBox)FindName("downloadPathBox")).Text = dialog.SelectedPath;
				}
			}
		}

		private void OpenTargetFolder(object sender, RoutedEventArgs e)
		{
			var folder = ((TextBox)FindName("downloadPathBox")).Text;
			if (!string.IsNullOrWhiteSpace(folder)){
				Process process = new Process();
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = "explorer.exe";
				process.StartInfo.Arguments = folder;
				process.Start();
			}
		}

		private async void StartDownload(object sender, RoutedEventArgs e)
		{	
			ComboBox bListDown = (ComboBox)FindName("cmbDownBranches");
			string branchName = (string)bListDown.SelectedItem;
			string folder = ((TextBox)FindName("downloadPathBox")).Text;
			bool createBranchFolder = ((CheckBox)FindName("chkCreateSub")).IsChecked == true;

			if (createBranchFolder){
				folder += $"/{branchName}/";
			}else{
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
			}else{
				LogActivity($"no code files downloaded");
			}
			
		}
	}	
}


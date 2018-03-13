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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ScreepsCodeCommiter
{
	/// <summary>
	/// This part contains all general things
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

		/// <summary>
		/// a fake event to prevent the wpf routing feature routung up combo box events to tabcontrol select
		/// </summary>
		private void DoNothing(object sender, RoutedEventArgs e)
		{
			//just to prevent the stupid WPF "routing feature"
			e.Handled = true;
		}

		/// <summary>
		/// folder for saving of config
		/// </summary>
		private string GetConfigSaveFolder()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			System.IO.Directory.CreateDirectory(path + @"\ScreepsCodeCommiter\");
			return path + @"\ScreepsCodeCommiter\";
		}

		/// <summary>
		/// being called when the user selects another tab
		/// </summary>0
		public void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var ctrl = (TabControl)sender;
			if (null != ctrl.SelectedItem)
			{
				var tab = (TabItem)ctrl?.SelectedItem;
				switch (tab?.Name)
				{
					case "tabDownload":
						((Grid)FindName("upGrid")).IsEnabled = false;
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
						if (null != serverList.SelectedItem)
						{
							ServerInfo serverInfo = (ServerInfo)serverList.SelectedItem;
							((Label)FindName("upStatus")).Content = $"Please wait, getting info from {serverInfo.Address}";
							UpdateDownloadTab(serverInfo);
						}
						else
						{
							((Label)FindName("downStatus")).Content = $"Please create and Select a server in settings tab.";
						}
						break;
					default:
						((Grid)FindName("upGrid")).IsEnabled = false;
						((Grid)FindName("downGrid")).IsEnabled = false;
						break;
				}
			}
		}

		/// <summary>
		/// clear the logger
		/// </summary>
		public void ClearLog(object sender, RoutedEventArgs e)
		{
			logBox.Document.Blocks.Clear();
		}

		/// <summary>
		/// adds an info to the logger
		/// </summary>
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

		/// <summary>
		/// opens the folder wich is the selected target for download
		/// </summary>
		private void OpenTargetFolder(object sender, RoutedEventArgs e)
		{
			var folder = ((TextBox)FindName("downloadPathBox")).Text;
			if (!string.IsNullOrWhiteSpace(folder))
			{
				Process process = new Process();
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = "explorer.exe";
				process.StartInfo.Arguments = folder;
				process.Start();
			}
		}
	}
}


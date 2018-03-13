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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ScreepsCodeCommiter
{
	/// <summary>
	/// This part contains all methods related for the code upload function
	/// </summary>
	public partial class MainWindow
	{

		bool createNewBranch = false;

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
			createNewBranch = false;
			ComboBox bListDown = FindName("cmbUpBranches") as ComboBox;
			bListDown.IsEnabled = true;
			TextBox textBox = FindName("txtNewBranch") as TextBox;
			textBox.IsEnabled = false;
		}
		/// <summary>
		/// on decision for existing branch
		/// </summary>
		private void RadioBranchSelectNew(object sender, RoutedEventArgs e)
		{
			createNewBranch = true;
			ComboBox bListDown = FindName("cmbUpBranches") as ComboBox;
			bListDown.IsEnabled = false;
			TextBox textBox = FindName("txtNewBranch") as TextBox;
			textBox.IsEnabled = true;
		}

		private void CheckUpload(object sender, RoutedEventArgs e)
		{
			(FindName("uploadFilesSubGrid") as Grid).IsEnabled = true;
		}

		private void UnCheckUpload(object sender, RoutedEventArgs e)
		{
			(FindName("uploadFilesSubGrid") as Grid).IsEnabled = true;
		}

		/// <summary>
		/// opens the folder wich is the selected target for download
		/// </summary>
		private void OpenUploadTargetFolder(object sender, RoutedEventArgs e)
		{
			var folder = ((TextBox)FindName("uploadPathBox")).Text;
			if (!string.IsNullOrWhiteSpace(folder))
			{
				Process process = new Process();
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = "explorer.exe";
				process.StartInfo.Arguments = folder;
				process.Start();
			}
		}

		/// <summary>
		/// force user to confirm this checkbox
		/// </summary>
		private void CheckConfirmUpload(object sender, RoutedEventArgs e)
		{
			var checkBox = (CheckBox)sender;
			var grid = FindName("uploadFilesSubGrid") as Grid;
			if (checkBox.IsChecked == true)
			{
				grid.IsEnabled = true;
			}
			else
			{
				grid.IsEnabled = false;
			}
		}

		/// <summary>
		/// select the folder to upload
		/// </summary>
		private void PickUploadTargetFolder(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					((TextBox)FindName("uploadPathBox")).Text = dialog.SelectedPath;
				}
			}
		}

		private void UploadBranch(object sender, RoutedEventArgs e)
		{
			var path = ((TextBox)FindName("uploadPathBox")).Text;
			string branch;
			if (createNewBranch)
			{
				TextBox textBox = FindName("txtNewBranch") as TextBox;
				branch = textBox.Text;
			}
			else
			{
				ComboBox bList = FindName("cmbUpBranches") as ComboBox;
				branch = (string)bList.SelectedItem;
			}
			if (CheckFolder(path))
			{
				LogActivity($"uploading to branch {branch}");
				var files = ScanFolder(path);
				var codeFiles = new List<ScreepsCodeFile>();
				foreach (var file in files)
				{
					LogActivity($"loading file {file}");
					using (StreamReader reader = new StreamReader($"{path}{file}"))
					{
						try
						{
							string fileContent = reader.ReadToEnd();
							codeFiles.Add(new ScreepsCodeFile(Path.GetFileName(file).Replace(".js", ""), fileContent));
						}
						catch (System.Exception)
						{
							LogActivity($"problem loading file {{}",true);
							throw;
						}
					}
				}
			}
			else
			{
				LogActivity($"upload canceled!");
			}
		}

		private void CheckUploadFolder(object sender, RoutedEventArgs e)
		{
			var path = ((TextBox)FindName("uploadPathBox")).Text;
			var resultInfo = FindName("checkResult") as Label;
			if (CheckFolder(path))
			{
				resultInfo.Content = "All seems to be fine for Upload";
			}
			else
			{
				resultInfo.Content = "there where problems, see log -->";
			}
		}

		/// <summary>
		/// check if there are double files
		/// </summary>
		private bool CheckFolder(string path)
		{
			var files = ScanFolder(path);
			files = files.Select(x => Path.GetFileName(x));
			var doubles = files.GroupBy(f => f).SelectMany(g => g.Skip(1));
			if (doubles.Count() > 0)
			{
				LogActivity($"error: there are files with the same file name", true);
				LogActivity($"its not possible to have 2 files with the same name!");
				LogActivity($"maybe the folder contains more than 1 branch?");

				string fileList = string.Join(", ", doubles.Select(x => x));
				LogActivity($"followinng files have ben found more than times::", true);
				LogActivity(fileList);
				return false;
			}
			foreach (var item in files)
			{
				System.Console.WriteLine(item);
			}
			LogActivity($"{files.Count()} checked, ok");
			return true;
		}

		private IEnumerable<string> ScanFolder(string path)
		{
			LogActivity($"scanning folder folder (and subfolders) for .js files...");
			var files = Directory.GetFiles(path, "*.js", SearchOption.AllDirectories);
			LogActivity($"found {files.Length} files");
			files = files.Select(x => { return x.Replace(path, ""); }).ToArray();
			return files;
		}
	}
}

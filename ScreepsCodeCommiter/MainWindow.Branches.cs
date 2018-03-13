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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScreepsCodeCommiter
{
	public partial class MainWindow
	{/// <summary>
	 /// refreshes the branch list into the given combo box
	 /// </summary>
		private async void RefreshBranchLists(ComboBox bListDown)
		{
			var branches = await GetBranches();
			if (null != branches)
			{
				//lets try to keep what was being selected before
				var prefSelection = bListDown.SelectedItem;
				bListDown.Items.Clear();
				foreach (string branch in branches)
				{
					bListDown.Items.Add(branch);
				}

				if (null != prefSelection && bListDown.Items.Contains(prefSelection))
				{
					//re- select prev selection
					bListDown.SelectedItem = prefSelection;
				}
				else
				{
					bListDown.SelectedItem = branches.First();
				}
				((Label)FindName("dwnActiveServer")).Content = branches.First();

			}
			else
			{
				return;
			}
		}

		/// <summary>
		/// download the branches
		/// </summary>
		private async Task<IEnumerable<string>> GetBranches()
		{
			LogActivity($"Downloading Branch List...", false);
			IEnumerable<string> branches = await currentConnector.GetBranches();
			if (null == branches)
			{
				LogActivity($"Could not download Branch List", true);
			}
			else
			{
				LogActivity($"found {branches.Count()} branch names");
			}
			return branches;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonRefreshBranches(object sender, RoutedEventArgs e)
		{
			ComboBox bListDown = (ComboBox)FindName("cmbDownBranches");
			RefreshBranchLists(bListDown);
		}
		private void PickDownlodTargetFolder(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					((TextBox)FindName("downloadPathBox")).Text = dialog.SelectedPath;
				}
			}
		}
	}
}

﻿/*Copyright (c) 2018 Annika Ryll License: MIT

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
using System.Windows.Controls.Primitives;

namespace ScreepsCodeCommiter
{
	public partial class MainWindow
	{
		/// <summary>
		/// refreshes the branch list into the given combo box
		/// </summary>
		private async void RefreshBranchLists(Selector bListDown)
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

		private void ButtonRefreshBranchesDown(object sender, RoutedEventArgs e)
		{
			Selector bListDown = (Selector)FindName("cmbDownBranches");
			RefreshBranchLists(bListDown);
		}

		private void ButtonRefreshBranchesUp(object sender, RoutedEventArgs e)
		{
			Selector bListDown = (Selector)FindName("cmbUpBranches");
			RefreshBranchLists(bListDown);
		}

		private void ButtonRefreshBranchesBranches(object sender, RoutedEventArgs e)
		{
			Selector bListDown = (Selector)FindName("listBranches");
			RefreshBranchLists(bListDown);
		}

		private async void ButtonDeleteBranch(object sender, RoutedEventArgs e)
		{
			Selector branchList = (Selector)FindName("listBranches");
			var branch = (string)branchList.SelectedItem;

			if (!string.IsNullOrWhiteSpace(branch))
			{
				bool report = await currentConnector.DeleteBranch(branch);
				if (report)
				{
					LogActivity($"branch {branch} was deleted");
				}
				else
				{
					LogActivity($"could not delete {branch}", true);
				}
				RefreshBranchLists(branchList);
			}
			else
			{
				LogActivity("please select a branch");
			}

			((CheckBox)FindName("deleteConfirmCheck")).IsChecked = false;
		}

		private async void ButtonNewBranch(object sender, RoutedEventArgs e)
		{
			Selector branchList = (Selector)FindName("listBranches");
			TextBox newNameBox = (TextBox)FindName("txtNewBranchName");
			var newName = newNameBox.Text;
			if (!string.IsNullOrWhiteSpace(newName))
			{
				bool report = await currentConnector.CloneBranch(null, newName);
				if (report)
				{
					LogActivity($"new empty branch {newName} created");
				}
				else
				{
					LogActivity($"could create the new branch :(", true);
				}
				RefreshBranchLists(branchList);
			}
			else
			{
				LogActivity("please select a branch");
			}
		}

		private async void ButtonCloneBranch(object sender, RoutedEventArgs e)
		{
			Selector branchList = (Selector)FindName("listBranches");
			var branch = (string)branchList.SelectedItem;
			TextBox newNameBox = (TextBox)FindName("txtNewBranchName");
			var newName = newNameBox.Text;
			if (!string.IsNullOrWhiteSpace(branch))
			{
				bool report = await currentConnector.CloneBranch(branch, newName);
				if (report)
				{
					LogActivity($"new branch {newName} cloned from {branch}");
				}
				else
				{
					LogActivity($"could not clone {newName} from {branch} :(", true);
				}
				RefreshBranchLists(branchList);
			}
			else
			{
				LogActivity("please select a branch");
			}
		}
	}
}

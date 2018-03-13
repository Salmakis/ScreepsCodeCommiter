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

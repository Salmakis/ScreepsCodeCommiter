using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ScreepsCodeCommiter
{
	/// <summary>
	/// Interaktionslogik für ServerWindow.xaml
	/// </summary>
	public partial class ServerWindow : Window
	{
		public ServerInfo SrvInfo { get; set; }

		public ServerWindow()
		{
			InitializeComponent();
		}

		public void SetInfo(ServerInfo info){
			this.SrvInfo = info;
			((TextBox)FindName("txtAddress")).Text = SrvInfo.Address;
			((TextBox)FindName("txtToken")).Text = SrvInfo.Token;
		}

		private void ButtonClose(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ButtonOk(object sender, RoutedEventArgs e)
		{

			SrvInfo.Address = ((TextBox)FindName("txtAddress")).Text;
			SrvInfo.Token = ((TextBox)FindName("txtToken")).Text;

			if (!string.IsNullOrWhiteSpace(SrvInfo.Address))
			{
				if (!ServerInfo.servers.Contains(SrvInfo)){
					ServerInfo.servers.Add(SrvInfo);
				}
				this.Close();
			}
		}
	}
}
using ScreepsConnection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

namespace ScreepsWpf
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		Connector screepsConnection = new Connector("05e59336-8feb-433c-b553-b97d260a5780");
		TextBox responceView;

		public MainWindow()
		{
			if (Debugger.IsAttached)
				CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

			InitializeComponent();
			responceView = (TextBox)FindName("OutResponce");
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var room = ((TextBox)FindName("InRoom")).Text;
			var shard = int.Parse(((TextBox)FindName("InShard")).Text);

			responceView.Text = (await screepsConnection.GetRoom(room, shard)).RoomName;
		}
	}
}

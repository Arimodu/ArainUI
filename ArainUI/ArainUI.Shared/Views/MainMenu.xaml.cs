using ArainUI.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArainUI 
{ 
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ConnectPage));
        }
        private void ManageBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PluginMangerPage));
        }
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TournamentAssistantShared;
using TournamentAssistantShared.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace ArainUI.Views
{
    public sealed partial class ConnectPage : Page
    {
        public ObservableCollection<ServerViewData> ScrapedServersData { get; private set; }
        public ConnectPage()
        {
            ScrapedServersData = new ObservableCollection<ServerViewData>();
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back) return;

            ServerScrapeProgress.Visibility = Visibility.Visible;

            Progress<int> progress = new Progress<int>(value =>
            {
                ServerScrapeProgress.Value = value;
            });

            //Start this as a task and let the UI finish its bussiness
            ScrapeServers(progress);
        }

        private void RefreshServerList_Click(object sender, RoutedEventArgs e)
        {
            ServerListViewBorder.Visibility = Visibility.Collapsed;
            ServerScrapeProgress.IsIndeterminate = true;
            ServerScrapeProgress.Value = 0;
            ServerScrapeProgress.Visibility = Visibility.Visible;
            ScrapedServersData.Clear();
            ServerAddressBox.Text = string.Empty;
            ServerPortBox.Text = string.Empty;

            Progress<int> progress = new Progress<int>(value =>
            {
                ServerScrapeProgress.Value = value;
            });

            ScrapeServers(progress);
        }

        private void ServerListView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedServerData = (sender as ListView).SelectedItem as ServerViewData;

            //For whatever reason this gets randomly fired when clearing the collection
            //Need to handle null -_-
            if (selectedServerData == null)
            {
                //If we have no server selected and password box is not visible, show password box
                if (PasswordBox.Visibility == Visibility.Collapsed) PasswordBox.Visibility = Visibility.Visible;
                return;
            }
            ServerAddressBox.Text = selectedServerData.Address;
            ServerPortBox.Text = selectedServerData.Port.ToString();

            //If selected server is password protected and passwordbox is not visible, show password box
            if (selectedServerData.IsPasswordProtected && PasswordBox.Visibility == Visibility.Collapsed)
            {
                UserPasswordGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                PasswordBox.Visibility = Visibility.Visible;
            }
                
            //If selected server is NOT password protected and password box is visible, hide password box
            if (!selectedServerData.IsPasswordProtected && PasswordBox.Visibility == Visibility.Visible)
            {
                UserPasswordGrid.ColumnDefinitions[1].Width = GridLength.Auto;
                PasswordBox.Visibility = Visibility.Collapsed;
            }
                
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            //If either address or port box is empty and server list selection is null -> play animation <This part not implemented yet> and return
            if ((string.IsNullOrEmpty(ServerAddressBox.Text) || string.IsNullOrEmpty(ServerPortBox.Text)) && ServerListView.SelectedItem is null)
            {
                return;
            }

            //If port box cannot be parsed as int -> play animation <This part not implemented yet> and return
            if (!Int32.TryParse(ServerPortBox.Text, out int portNumber))
            {
                return;
            }

            //If selected item address is not the same as whats in the address box or selected item port is not the same as whats in the port box, use content of address / port box
            //Assuming no password, at this point idk how to deal with that, but lets cross that bridge when we get there
            //Well it wasnt that difficult afterall, I just had to sleep on it...
            if ((ServerListView.SelectedItem as ServerViewData).Address.ToLower().Trim() != ServerAddressBox.Text.ToLower().Trim() || (ServerListView.SelectedItem as ServerViewData).Port != portNumber)
            {
                ConnectToServer(ServerAddressBox.Text, portNumber, string.IsNullOrEmpty(UsernameBox.Text.Trim()) ? "Coordinator" : UsernameBox.Text, PasswordBox.Text);
            }

            //If none of the above catch we can safely assume we can use the selected item
            ConnectToServer((ServerListView.SelectedItem as ServerViewData).Address, portNumber, string.IsNullOrEmpty(UsernameBox.Text.Trim()) ? "Coordinator" : UsernameBox.Text, PasswordBox.Text);
        }

        private void ConnectToServer(string address, int port, string username, string password = null)
        {
            Frame.Navigate(typeof(ServerHubPage), new SystemClient(address, port, username, User.ClientTypes.Coordinator, null, password));
        }

        private async void ScrapeServers(IProgress<int> progress)
        {
            //Assuming we dont know anything, lets scrape the masterserver first
            var masterServer = new CoreServer
            {
                Address = Constants.MASTER_SERVER,
                Port = 2052
            };
            var masterState = await HostScraper.ScrapeHost(masterServer, "ArainUI", 0);

            ServerScrapeProgress.IsIndeterminate = false;
            ServerScrapeProgress.Maximum = masterState.KnownHosts.Count;

            var scraped = await HostScraper.ScrapeHosts(masterState.KnownHosts.ToArray(), "ArainUI", 0, null, (server, state, position, total) => { progress.Report(position); });

            //This is here until Moon adds IsPasswordProtected to CoreServer model
            //I dont even want to look at this hackjob
            for (int i = 0; i < masterState.KnownHosts.Count; i++)
            {
                var server = masterState.KnownHosts[i];
                try
                {
                    _ = scraped[server];
                    ScrapedServersData.Add(new ServerViewData(server, false));
                }
                catch (KeyNotFoundException)
                {
                    ScrapedServersData.Add(new ServerViewData(server, true));
                }
            }

            ServerScrapeProgress.Visibility = Visibility.Collapsed;
            ServerListViewBorder.Visibility = Visibility.Visible;
        }
    }

    //This is here until Moon adds IsPasswordProtected to CoreServer model
    public class ServerViewData : CoreServer
    {
        public bool IsPasswordProtected { get; private set; }
        public ServerViewData(CoreServer coreServer, bool isPasswordProtected)
        {
            Name = coreServer.Name;
            Address = coreServer.Address;
            Port = coreServer.Port;
            IsPasswordProtected = isPasswordProtected;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TournamentAssistantShared;
using TournamentAssistantShared.Models;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using System.Collections.ObjectModel;

namespace ArainUI.Views
{
    public sealed partial class ServerHubPage : Page
    {
        public ObservableCollection<User> AvailablePlayers { get; private set; } = new ObservableCollection<User>();
        public ObservableCollection<User> SelectedUsers { get; private set; } = new ObservableCollection<User>();
        public SystemClient Client { get; private set; }
        public ServerHubPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter as SystemClient != null)
            {
                Client = e.Parameter as SystemClient;

                Client.ConnectedToServer += Client_ConnectedToServer;
                Client.FailedToConnectToServer += Client_FailedToConnectToServer;
                Client.ServerDisconnected += Client_ServerDisconnected;

                Task.Run(Client.Start);
            } 
            base.OnNavigatedTo(e);
        }

        private Task Client_ServerDisconnected()
        {
            throw new NotImplementedException();
        }

        private async Task Client_FailedToConnectToServer(TournamentAssistantShared.Models.Packets.ConnectResponse arg)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() => 
            {
                ConnectionProgressBar.IsIndeterminate = false;
                ConnectionProgressBar.Maximum = 100;
                ConnectionProgressBar.Value = 0;

                //Run a timeout, then navigate back to ConnectPage
                Task.Run(async () =>
                {
                    for (int i = 0; i <= 100; i++)
                    {
                        await Dispatcher.RunIdleAsync(new Windows.UI.Core.IdleDispatchedHandler((e) =>
                        {
                            ConnectionProgressBar.Value = i;
                        }));
                        await Task.Delay(30);
                    }
                    
                    await Dispatcher.RunIdleAsync(new Windows.UI.Core.IdleDispatchedHandler((e) =>
                    {
                        Frame.GoBack();
                    }));
                });

                ResourceLoader resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                ConnectionStatusBlock.Text = resourceLoader.GetString("HubPageConnectionInfo/ConnectionFailed");

                ConnectionDetailsBlock.Text = arg.Response.Message;
                ConnectionDetailsBlock.Visibility = Visibility.Visible;
            }));
        }

        private async Task Client_ConnectedToServer(TournamentAssistantShared.Models.Packets.ConnectResponse arg)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                ConnectionProgressBar.IsIndeterminate = false;
                ConnectionProgressBar.Maximum = 100;
                ConnectionProgressBar.Value = 0;

                //Run a timeout, then view the hub
                Task.Run(async () =>
                {
                    for (int i = 0; i <= 100; i++)
                    {
                        await Dispatcher.RunIdleAsync(new Windows.UI.Core.IdleDispatchedHandler((e) =>
                        {
                            ConnectionProgressBar.Value = i;
                        }));
                        await Task.Delay(10);
                    }

                    await Dispatcher.RunIdleAsync(new Windows.UI.Core.IdleDispatchedHandler((e) =>
                    {
                        ServerConnectionGrid.Visibility = Visibility.Collapsed;
                        HubGrid.Visibility = Visibility.Visible;
                    }));
                });

                ResourceLoader resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                ConnectionStatusBlock.Text = resourceLoader.GetString("HubPageConnectionInfo/ConnectionSuccess");
            }));
        }
    }
}

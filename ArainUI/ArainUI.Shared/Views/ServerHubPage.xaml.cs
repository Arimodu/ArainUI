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
using Windows.UI.Xaml.Media.Imaging;

namespace ArainUI.Views
{
    public sealed partial class ServerHubPage : Page
    {
        public ObservableCollection<User> AvailablePlayers { get; private set; } = new ObservableCollection<User>();
        public ObservableCollection<User> AvailableCoordinators { get; private set; } = new ObservableCollection<User>();
        public ObservableCollection<User> SelectedPlayers { get; private set; } = new ObservableCollection<User>();
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

                Client.UserConnected += Client_UserConnected;
                Client.UserDisconnected += Client_UserDisconnected;
                Client.UserInfoUpdated += Client_UserInfoUpdated;

                Task.Run(Client.Start);
            } 
            base.OnNavigatedTo(e);
        }

        private async Task Client_UserInfoUpdated(User user)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                if (AvailablePlayers.Any((player) => player.Id == user.Id)) AvailablePlayers[AvailablePlayers.IndexOf(user)] = user;
                if (SelectedPlayers.Any((player) => player.Id == user.Id)) SelectedPlayers[SelectedPlayers.IndexOf(user)] = user;
                if (AvailableCoordinators.Any((coordinator) => coordinator.Id == user.Id)) AvailableCoordinators[AvailableCoordinators.IndexOf(user)] = user;
            }));
        }

        private async Task Client_UserDisconnected(User user)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                if (AvailablePlayers.Any((player) => player.Id == user.Id)) AvailablePlayers.Remove(user);
                if (SelectedPlayers.Any((player) => player.Id == user.Id)) SelectedPlayers.Remove(user);
                if (AvailableCoordinators.Any((coordinator) => coordinator.Id == user.Id)) AvailableCoordinators.Remove(user);
            }));
        }

        private async Task Client_UserConnected(User user)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                switch (user.ClientType)
                {
                    case User.ClientTypes.Player:
                        AvailablePlayers.Add(user);
                        break;
                    case User.ClientTypes.Coordinator:
                        AvailableCoordinators.Add(user);
                        break;
                    case User.ClientTypes.TemporaryConnection:
                        break;
                    case User.ClientTypes.WebsocketConnection:
                        break;
                    default:
                        break;
                }
            }));
        }

        private Task Client_ServerDisconnected()
        {
            return Task.CompletedTask;
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

        private async Task Client_ConnectedToServer(TournamentAssistantShared.Models.Packets.ConnectResponse response)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                //Select users where user is player and not in any match, in case no matches are open select all connected users and create a new instance of ObservableCollection
                AvailablePlayers = new ObservableCollection<User>(response.State.Users.Where((user) => user.ClientType == User.ClientTypes.Player && response.State.Matches.All((match) => !match.AssociatedUsers.Contains(user))));

                AvailableCoordinators = new ObservableCollection<User>(response.State.Users.Where((user) => user.ClientType == User.ClientTypes.Coordinator && response.State.Matches.All((match) => !match.AssociatedUsers.Contains(user))));

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

                //Select users where user is player and not in any match, in case no matches are open select all connected users
                //Also check if player hasn't been already added by another event
                foreach (var player in response.State.Users.Where((user) => user.ClientType == User.ClientTypes.Player && response.State.Matches.All((match) => !match.AssociatedUsers.Contains(user))))
                {
                    if (AvailablePlayers.Any((user) => user.Id == player.Id)) continue;
                    AvailablePlayers.Add(player);
                }

                foreach (var coordinator in response.State.Users.Where((user) => user.ClientType == User.ClientTypes.Coordinator))
                {
                    if (AvailableCoordinators.Any((user) => user.Id == coordinator.Id)) continue;
                    AvailableCoordinators.Add(coordinator);
                }
            }));
        }
    }
}

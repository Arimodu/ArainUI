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

namespace ArainUI.Views
{
    public sealed partial class ServerHubPage : Page
    {
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

        private Task Client_FailedToConnectToServer(TournamentAssistantShared.Models.Packets.ConnectResponse arg)
        {
            throw new NotImplementedException();
        }

        private Task Client_ConnectedToServer(TournamentAssistantShared.Models.Packets.ConnectResponse arg)
        {
            Client.Shutdown();
            return Task.CompletedTask;
        }
    }
}

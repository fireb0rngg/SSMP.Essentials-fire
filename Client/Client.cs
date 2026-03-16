using SSMP.Api.Client;
using SSMPUtils.Client.Commands;
using SSMPUtils.Client.Modules;
using SSMPUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMPUtils.Client
{
    internal class Client : ClientAddon
    {
        protected override string Name => "SSMP Utils";
        protected override string Version => SSMPUtilsPlugin.Version;
        public override uint ApiVersion => Config.SSMPApiVersion;
        public override bool NeedsNetwork => true;

        internal static IClientApi api;

        internal static Client instance;

        public override void Initialize(IClientApi clientApi)
        {
            instance = this;
            api = clientApi;

            PacketReceiver.Init();
            PacketSender.Init();

            api.CommandManager.RegisterCommand(new Huddle());

            api.ClientManager.DisconnectEvent += () => Spectate.ReturnToSelf();

            Spectate.Init();

            Log.LogInfo("Utils Client Initialized");

        }

        public static void LocalChat(string message)
        {
            api.UiManager.ChatBox.AddMessage(message);
        }

        public static IClientPlayer? GetPlayerByName(string username)
        {
            // Try to find player
            var players = Client.api.ClientManager.Players;
            var foundPlayer = players.FirstOrDefault(p => p.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));

            return foundPlayer;
        }
    }
}

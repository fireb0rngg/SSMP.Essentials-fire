using SSMP.Api.Client;
using SSMPUtils.Client.Commands;
using SSMPUtils.Client.Modules;
using SSMPUtils.Server.Modules;
using SSMPUtils.Utils;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace SSMPUtils.Client
{
    internal class Client : ClientAddon
    {
        protected override string Name => "SSMP Utils";
        protected override string Version => SSMPEssentialsPlugin.Version;
        public override uint ApiVersion => Config.SSMPApiVersion;
        public override bool NeedsNetwork => true;

        internal static IClientApi api;

        internal static Client instance;

        internal static ServerSettings ServerSettings = new(true);

        internal static Action OnServerSettingsUpdate = () => { };

        public override void Initialize(IClientApi clientApi)
        {

            instance = this;
            api = clientApi;

            Log.SetLogger(Logger);
            PacketReceiver.Init();
            PacketSender.Init();

            api.CommandManager.RegisterCommand(new Huddle());
            api.CommandManager.RegisterCommand(new TeleportRequest());
            api.CommandManager.RegisterCommand(new TeleportAccept());
            api.CommandManager.RegisterCommand(new TeleportDeny());

            api.ClientManager.DisconnectEvent += () => Spectate.ReturnToSelf();
            api.ClientManager.PlayerEnterSceneEvent += PlayerHealth.OnPlayerEnter;

            Spectate.Init();

            Log.LogInfo("SSMP Essentials Client Initialized");

        }

        public static void LocalChat(string message)
        {
            SSMPEssentialsPlugin.NextFrames.Add(() => api.UiManager.ChatBox.AddMessage(message));
        }

        public static IClientPlayer? GetPlayerByName(string username)
        {
            // Try to find player
            var players = api.ClientManager.Players;
            var foundPlayer = players.FirstOrDefault(p => p.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));

            return foundPlayer;
        }

        public static IClientPlayer? GetPlayer(ushort id)
        {
            return api.ClientManager.GetPlayer(id);
        }
    }
}

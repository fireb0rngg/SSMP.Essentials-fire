using System;
using System.Linq;
using SSMP.Api.Client;
using SSMPEssentials.Client.Modules;
using SSMPEssentials.Server.Modules;
using SSMPEssentials.Client.Commands;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Client
{
    internal class Client : ClientAddon
    {
        protected override string Name => Config.ModName;
        protected override string Version => SSMPEssentialsPlugin.Version;
        public override uint ApiVersion => Config.SSMPApiVersion;
        public override bool NeedsNetwork => true;

        internal static IClientApi api;

        internal static Client instance;

        internal static ServerSettings ServerSettings = ServerSettings.ReadFromFile();

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
            api.CommandManager.RegisterCommand(new TeleportBack());

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

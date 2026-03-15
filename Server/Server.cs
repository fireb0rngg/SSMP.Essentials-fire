using System;
using System.Collections.Generic;
using System.Text;
using SSMP.Api.Server;
using SSMP_Utils.Utils;

namespace SSMP_Utils.Server
{
    internal class Server : ServerAddon
    {
        protected override string Name => "SSMP Utils";
        protected override string Version => SSMP_UtilsPlugin.Version;
        public override uint ApiVersion => Config.SSMPApiVersion;
        public override bool NeedsNetwork => true;

        internal static IServerApi api;

        internal static Server instance;

        public override void Initialize(IServerApi serverApi)
        {
            instance = this;
            api = serverApi;

            PacketReceiver.Init();
            PacketSender.Init();
            Log.LogInfo("Utils Server Initialized");
        }

        public static IServerPlayer? GetPlayer(ushort id)
        {
            return api.ServerManager.GetPlayer(id);
        }

    }
}

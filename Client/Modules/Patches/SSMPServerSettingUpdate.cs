using HarmonyLib;
using SSMP.Api.Client;
using SSMP.Game.Settings;
using SSMP.Networking.Packet;
using SSMPEssentials.Utils;
using System.Reflection;


/**
 * 
 * These patches will not be needed after the next version of SSMP
 * 
 */

namespace SSMPEssentials.Client.Modules.Patches
{
    [HarmonyPatch]
    internal class SSMPServerSettingUpdate
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("SSMP.Networking.Packet.Data.ServerSettingsUpdate");
            if (type == null)
            {
                Log.LogError("Unable to receive SSMP server setting updates (type)");
                return null;
            }

            var method = AccessTools.Method(type, nameof(IPacketData.ReadData));
            if (method == null)
            {
                Log.LogError("Unable to receive SSMP server setting updates (method)");
                return null;
            }

            return method;
        }
        [HarmonyPostfix]
        private static void Postfix(object __instance)
        {
            var settings = Traverse.Create(__instance).Property("ServerSettings").GetValue<ServerSettings>();

            Client.OnSSMPSettingsUpdate?.Invoke(settings);
        }
    }

    [HarmonyPatch]
    internal class SSMPTeamUpdate
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("SSMP.Game.Client.ClientPlayerData");
            if (type == null)
            {
                Log.LogError("Unable to receive SSMP team updates (type)");
                return null;
            }

            var method = AccessTools.PropertySetter(type, nameof(IClientPlayer.Team));
            if (method == null)
            {
                Log.LogError("Unable to receive SSMP team updates (method)");
                return null;
            }

            return method;
        }
        [HarmonyPostfix]
        public static void Postfix(IClientPlayer __instance)
        {
            var player = __instance;
            if (!player.IsInLocalScene) return;
            
            var team = player.Team;
            var self = Client.api.ClientManager;

            if (self.Team == team) Spectate.AddPlayer(player);
            else Spectate.RemovePlayer(player);
        }
    }
}

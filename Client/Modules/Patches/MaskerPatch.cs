using HarmonyLib;
using UnityEngine;

namespace SSMPEssentials.Client.Modules.Patches
{
    [HarmonyPatch]
    internal class MaskerPatch
    {
        [HarmonyPatch(typeof(Remasker), nameof(Remasker.OnTriggerEnter2D))]
        [HarmonyPrefix]
        public static bool OnTriggerEnter2D(Remasker __instance, Collider2D collision)
        {
            if (Spectate.FollowedPlayer != null && collision.gameObject == Spectate.FollowedPlayer)
            {
                __instance.Entered();
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Remasker), nameof(Remasker.OnTriggerExit2D))]
        [HarmonyPrefix]
        public static bool OnTriggerExit2D(Remasker __instance, Collider2D collision)
        {
            if (Spectate.FollowedPlayer != null && collision.gameObject == Spectate.FollowedPlayer)
            {
                __instance.Exited(false);
                return false;
            }

            return true;
        }
    }
}

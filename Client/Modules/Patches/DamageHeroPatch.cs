using HarmonyLib;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Client.Modules.Patches
{
    [HarmonyPatch]
    internal class DamageHeroPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DamageHero), nameof(DamageHero.OnAwake))]
        public static void DamageHeroAwake(DamageHero __instance)
        {
            __instance.HeroDamaged += () => PlayerDeaths.DetermineCauseOfDamage(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HeroController), nameof(HeroController.instance.DoSpecialDamage))]
        public static void DoSpecialDamage(HeroController __instance, bool isFrostDamage)
        {
            bool canDoDamage = !__instance.takeNoDamage && CheatManager.Invincibility == CheatManager.InvincibilityStates.Off && ToolItemManager.ActiveState != ToolsActiveStates.Cutscene && !__instance.cState.transitioning;
            if (!canDoDamage) return;

            if (isFrostDamage)
            {
                Log.LogInfo("Froze");
                PlayerDeaths.LatestCause = CauseOfDeath.Frost;
            }
            else
            {
                Log.LogInfo("Generic Damage Type");
                PlayerDeaths.LatestCause = CauseOfDeath.Generic;
            }
        }
    }
}

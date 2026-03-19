using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMPUtils.Client.Modules.Patches
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
    }
}

using HarmonyLib;

namespace SSMPEssentials.Client.Modules.Patches
{
    [HarmonyPatch]
    class HealthPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.AddHealth))]
        public static void AddHealth(PlayerData __instance)
        {
            PlayerHealth.OnHealthChange();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.TakeHealth))]
        public static void TakeHealth(PlayerData __instance)
        {
            PlayerHealth.OnHealthChange();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.AddToMaxHealth))]
        public static void AddToMaxHealth(PlayerData __instance)
        {
            PlayerHealth.OnHealthChange();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.MaxHealth))]
        public static void MaxHealth(PlayerData __instance)
        {
            PlayerHealth.OnHealthChange();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HeroController), nameof(HeroController.UpdateBlueHealth))]
        public static void UpdateBlueHealth(PlayerData __instance)
        {
            PlayerHealth.OnHealthChange();
        }
    }
}

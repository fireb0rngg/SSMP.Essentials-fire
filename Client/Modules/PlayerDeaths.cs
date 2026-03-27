using GlobalEnums;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using SSMPUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Rendering;

namespace SSMPUtils.Client.Modules
{
    internal static class PlayerDeaths
    {
        public static CauseOfDeath LatestCause = CauseOfDeath.Generic;
        public static ushort LatestPlayerAttack = 0;
        public static DateTime PlayerAttackTime = DateTime.MinValue;

        public static void OnDeath()
        {
            if (!Client.ServerSettings.DeathMessagesEnabled) return;

            var ranAway = false;
            if (PlayerAttackTime.AddSeconds(15) > DateTime.Now)
            {
                ranAway = true;
            }
            PacketSender.SendDeath(LatestCause, LatestPlayerAttack, ranAway);
        }

        public static void DetermineCauseOfDamage(DamageHero damager)
        {
            bool isPlayer = false;
            var parent = damager.transform;
            while (parent != null && !isPlayer)
            {
                if (parent.name == "Player Prefab") isPlayer = true;
                else parent = parent.parent;
            }

            Log.LogDebug(isPlayer, damager.name);
            if (isPlayer && parent)
            {
                LatestCause = CauseOfDeath.Player;

                var player = Client.api.ClientManager.Players.FirstOrDefault(p => p.PlayerObject == parent.gameObject);
                if (player == null)
                {
                    Log.LogWarning("Attacked by unknown player");
                    return;
                }
                LatestPlayerAttack = player?.Id ?? 0;
                PlayerAttackTime = DateTime.Now;
                return;
            }

            LatestCause = DetermineHazardType(damager.hazardType);
            Log.LogDebug(LatestCause);
        }

        static CauseOfDeath DetermineHazardType(HazardType hazardType)
        {
            return hazardType switch
            {
                HazardType.ENEMY => CauseOfDeath.Enemy,
                HazardType.SPIKES => CauseOfDeath.Spikes,
                HazardType.ACID => CauseOfDeath.Acid,
                HazardType.LAVA => CauseOfDeath.Lava,
                HazardType.PIT => CauseOfDeath.Pit,
                HazardType.COAL => CauseOfDeath.Coal,
                HazardType.ZAP => CauseOfDeath.Zap,
                HazardType.EXPLOSION => CauseOfDeath.Explosion,
                HazardType.SINK => CauseOfDeath.Sink,
                HazardType.STEAM => CauseOfDeath.Steam,
                HazardType.RESPAWN_PIT => CauseOfDeath.Pit,
                _ => CauseOfDeath.Generic
            };
        }
    }
}

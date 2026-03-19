using SSMP.Api.Server;
using SSMP.Api.Server.Networking;
using SSMPUtils.Utils;
using SSMPUtils.Client.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using SSMP.Math;
using Cause = SSMPUtils.Client.Modules.PlayerDeaths.CauseOfDeath;

namespace SSMPUtils.Server
{
    internal static class PacketReceiver
    {
        static IServerAddonNetworkReceiver<PacketIDs> receiver;

        public static void Init()
        {
            receiver = Server.api.NetServer.GetNetworkReceiver<PacketIDs>(Server.instance, Client.Packets.Packets.Instantiate);
            receiver.RegisterPacketHandler<TeleportPacket>(PacketIDs.Huddle, OnHuddle);
            receiver.RegisterPacketHandler<TeleportRequestPacket>(PacketIDs.TeleportRequest, OnTeleportRequest);
            receiver.RegisterPacketHandler<TeleportPacket>(PacketIDs.TeleportAccept, OnTeleportRequestAccept);
            receiver.RegisterPacketHandler<MessagePacket>(PacketIDs.Message, OnMessage);
            receiver.RegisterPacketHandler<DeathPacket>(PacketIDs.PlayerDeath, OnPlayerDeath);
        }

        public static void OnHuddle(ushort id, TeleportPacket data)
        {
            // Check if authorized
            var player = Server.GetPlayer(id);
            if (player == null || !player.IsAuthorized)
            {
                Server.SendMessageToPlayer(id, "You need to be authorized to use that command.");
                return;
            }

            // No scene = teleporting to someone else
            if (data.Scene == "")
            {
                var targetPlayer = Server.GetPlayer(data.PlayerId);
                if (targetPlayer == null)
                {
                    Server.SendMessageToPlayer(id, "I couldn't find that player.");
                    return;
                }

                data.Scene = targetPlayer.CurrentScene;
                data.Position = targetPlayer.Position ?? Vector2.Zero;
            }
            else
            {
                data.PlayerId = id;
            }

            PacketSender.BroadcastWarp(data.Scene, data.Position, data.PlayerId);
        }

        public static void OnTeleportRequest(ushort id, TeleportRequestPacket data)
        {
            var targetPlayer = Server.GetPlayer(data.PlayerId);
            if (targetPlayer == null)
            {
                Server.SendMessageToPlayer(id, "I couldn't find that player.");
                return;
            }

            PacketSender.SendTeleportRequest(data.PlayerId, id);
        }

        public static void OnTeleportRequestAccept(ushort id, TeleportPacket data)
        {
            var playerToTeleport = Server.GetPlayer(data.PlayerId);
            if (playerToTeleport == null)
            {
                Server.SendMessageToPlayer(id, "I couldn't find that player.");
                return;
            }

            PacketSender.SendTeleportAccepted(data.PlayerId, data.Scene, data.Position);
        }

        public static void OnMessage(ushort id, MessagePacket data)
        {
            var player = Server.GetPlayer(data.PlayerId);
            if (player == null)
            {
                Server.SendMessageToPlayer(id, "I couldn't find that player.");
                return;
            }

            PacketSender.SendMessage(data.PlayerId, id, data.Message);
        }

        public static void OnPlayerDeath(ushort id, DeathPacket data)
        {
            var player = Server.GetPlayer(id);
            if (player == null) return;

            var opponent = Server.GetPlayer(data.KillerID)?.Username ?? "another user";
            var deathString = player.Username + " " + DetermineDeathString(data.Cause, opponent);

            if (data.Cause != Cause.Player && data.RanAway)
            {
                deathString += " while running away from " + opponent;
            }

            Server.api.ServerManager.BroadcastMessage(deathString);
        }

        static string DetermineDeathString(Cause cause, string username)
        {
            return cause switch
            {
                Cause.Player => $"lost a duel to {username}",
                Cause.Enemy => "had an intimate encounter with an enemy",
                Cause.Spikes => "got impaled by spikes",
                Cause.Acid => "forgot to equip Isma's Tear before hopping in acid",
                Cause.Lava => "sung Lava Chicken too hard and became one",
                Cause.Pit => "fell down a hole",
                Cause.Coal => "got hit by a lump of coal in the head",
                Cause.Zap => "was electrocuted to death",
                Cause.Explosion => "went Last Judge mode and blew up",
                Cause.Sink => "sunk so low that they couldn't get out",
                Cause.Steam => "took a ride in a kettle",
                Cause.CoalSpikes => "did something with coal spikes (not sure what that is)",
                _ => "died so epicly that there isn't even a reason",
            };
        }
    }
}

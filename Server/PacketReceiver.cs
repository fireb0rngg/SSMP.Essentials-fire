using SSMP.Api.Server;
using SSMP.Api.Server.Networking;
using SSMPUtils.Utils;
using SSMPUtils.Client.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using SSMP.Math;

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
    }
}

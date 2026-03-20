using SSMP.Api.Client.Networking;
using SSMPUtils.Utils;
using SSMPUtils.Server.Packets;
using SSMPUtils.Client.Modules;
using UnityEngine;
using SSMPUtils.Client.Packets;
using System;

namespace SSMPUtils.Client
{
    internal static class PacketReceiver
    {
        static IClientAddonNetworkReceiver<PacketIDs> receiver;
        public static void Init()
        {
            receiver = Client.api.NetClient.GetNetworkReceiver<PacketIDs>(Client.instance, Server.Packets.Packets.Instantiate);
            receiver.RegisterPacketHandler<TeleportPacket>(PacketIDs.Huddle, OnHuddle);
            receiver.RegisterPacketHandler<TeleportRequestPacket>(PacketIDs.TeleportRequest, OnTeleportRequest);
            receiver.RegisterPacketHandler<TeleportPacket>(PacketIDs.TeleportAccept, OnTeleportAccepted);
            receiver.RegisterPacketHandler<MessagePacket>(PacketIDs.Message, OnMessage);
        }

        public static void OnHuddle(TeleportPacket data)
        {
            var huddleWarp = new Warp(data.Scene, (Vector2)data.Position);
            huddleWarp.WarpToPosition();
        }

        public static void OnTeleportRequest(TeleportRequestPacket data)
        {
            var player = Client.api.ClientManager.GetPlayer(data.PlayerId);
            if (player == null)
            {
                Client.LocalChat("Received teleport request from unknown player. Ignoring.");
                return;
            }

            TeleportRequests.AddRequest(player);
        }

        public static void OnTeleportAccepted(TeleportPacket data)
        {
            Client.LocalChat("Teleport request accepted. Teleporting now...");
            var teleportWarp = new Warp(data.Scene, (Vector2)data.Position);
            teleportWarp.WarpToPosition();
        }

        public static void OnMessage(MessagePacket data)
        {
            var player = Client.api.ClientManager.GetPlayer(data.PlayerId);
            var message = data.Message switch
            {
                Messages.TeleportDenied => $"{Common.ColoredUsername(player)} denied your teleport request.",
                _ => throw new NotImplementedException()
            };

            Client.LocalChat(message);
        }
    }
}

using SSMP.Api.Client.Networking;
using SSMPUtils.Utils;
using SSMPUtils.Server.Packets;
using SSMPUtils.Client.Packets;
using SSMPUtils.Client.Modules;
using UnityEngine;
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
            receiver.RegisterPacketHandler<PlayerHealthPacket>(PacketIDs.PlayerHealth, OnHealth);
            receiver.RegisterPacketHandler<SettingsPacket>(PacketIDs.Settings, OnSettings);
        }

        public static void OnHuddle(TeleportPacket data)
        {
            var huddleWarp = new Warp(data.Scene, (Vector2)data.Position);
            huddleWarp.WarpToPosition();
        }

        public static void OnTeleportRequest(TeleportRequestPacket data)
        {
            var player = Client.GetPlayer(data.PlayerId);
            if (player == null)
            {
                Client.LocalChat("Received teleport request from unknown player. Ignoring.");
                return;
            }

            if (!Client.ServerSettings.TeleportsNeedRequests)
            {
                var request = new TeleportRequests.Request(data.PlayerId);
                PacketSender.SendTeleportAccept(request);
                return;
            }

            TeleportRequests.AddRequest(player);
        }

        public static void OnTeleportAccepted(TeleportPacket data)
        {
            var player = Client.GetPlayer(data.PlayerId);
            Client.LocalChat($"Teleport request accepted. Teleporting to {Common.ColoredUsername(player)} now...");

            var teleportWarp = new Warp(data.Scene, (Vector2)data.Position);
            teleportWarp.WarpToPosition();
        }

        public static void OnMessage(MessagePacket data)
        {
            var player = Client.GetPlayer(data.PlayerId);
            var message = data.Message switch
            {
                Messages.TeleportDenied => $"{Common.ColoredUsername(player)} denied your teleport request.",
                _ => throw new NotImplementedException()
            };

            Client.LocalChat(message);
        }

        public static void OnHealth(PlayerHealthPacket data)
        {
            var player = Client.GetPlayer(data.PlayerId);
            if (player == null) return;

            PlayerHealth.SetPlayerHealth(player, data.Health);
        }

        public static void OnSettings(SettingsPacket data)
        {
            Client.ServerSettings = data.ServerSettings;
            Client.OnServerSettingsUpdate.Invoke();
        }
    }
}

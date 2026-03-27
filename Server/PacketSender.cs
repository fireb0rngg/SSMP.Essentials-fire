using SSMP.Api.Server.Networking;
using SSMP.Math;
using SSMPUtils.Data;
using SSMPUtils.Utils;

namespace SSMPUtils.Server
{
    internal static class PacketSender
    {
        static IServerAddonNetworkSender<PacketIDs> sender;
        internal static void Init()
        {
            sender = Server.api.NetServer.GetNetworkSender<PacketIDs>(Server.instance);
        }

        static void Broadcast(PacketIDs id, Packet packet, ushort senderId, bool collection = true)
        {
            var players = Server.api.ServerManager.Players;
            foreach (var player in players)
            {
                if (player.Id == senderId) continue;

                if (collection) sender.SendCollectionData(id, packet, player.Id);
                else sender.SendSingleData(id, packet, player.Id);
            }
        }

        internal static void BroadcastWarp(string scene, Vector2 position, ushort destinationPlayerId)
        {
            var data = new Client.Packets.TeleportPacket
            {
                Scene = scene,
                Position = position
            };

            Broadcast(PacketIDs.Huddle, data, destinationPlayerId, false);
        }

        internal static void SendTeleportRequest(ushort targetPlayerId, ushort senderId)
        {
            var data = new Client.Packets.TeleportRequestPacket
            {
                PlayerId = senderId
            };

            sender.SendCollectionData(PacketIDs.TeleportRequest, data, targetPlayerId);
        }

        internal static void SendTeleportAccepted(ushort playerToTeleportId, string scene,  Vector2 position)
        {
            var data = new Client.Packets.TeleportPacket
            {
                PlayerId = playerToTeleportId,
                Position = position,
                Scene = scene
            };

            sender.SendSingleData(PacketIDs.TeleportAccept, data, playerToTeleportId);
        }

        internal static void SendMessage(ushort recipientId, ushort senderId, Messages message)
        {
            var data = new Client.Packets.MessagePacket
            {
                Message = message,
                PlayerId = senderId
            };

            sender.SendCollectionData(PacketIDs.Message, data, recipientId);
        }

        internal static void BroadcastPlayerHealth(ushort playerId, HealthData health)
        {
            var data = new Packets.PlayerHealthPacket
            {
                PlayerId = playerId,
                Health = health
            };

            Broadcast(PacketIDs.PlayerHealth, data, playerId, true);
        }

        internal static void SendAllPlayerHealth(ushort recipientId)
        {
            foreach (var player in PlayerDataTracker.ServerInstance.GetAllData())
            {
                var data = new Packets.PlayerHealthPacket
                {
                    PlayerId = player.Id,
                    Health = player.Health
                };

                sender.SendCollectionData(PacketIDs.PlayerHealth, data, recipientId);
            }
        }

        internal static void BroadcastSettingsUpdate()
        {
            var data = new Packets.SettingsPacket
            {
                ServerSettings = Server.ServerSettings
            };

            sender.BroadcastSingleData(PacketIDs.Settings, data);
        }

        internal static void SendSettingsUpdate(ushort id)
        {
            var data = new Packets.SettingsPacket
            {
                ServerSettings = Server.ServerSettings
            };

            sender.SendSingleData(PacketIDs.Settings, data, id);
        }
    }
}

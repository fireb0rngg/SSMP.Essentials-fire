using SSMP.Api.Server.Networking;
using SSMP.Math;
using SSMP.Networking.Packet;
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

        static void Broadcast(PacketIDs id, IPacketData packet, ushort senderId)
        {
            var players = Server.api.ServerManager.Players;
            foreach (var player in players)
            {
                sender.SendSingleData(id, packet, player.Id);
            }
        }

        internal static void BroadcastWarp(string scene, Vector2 position, ushort senderId)
        {
            Broadcast(PacketIDs.Huddle, new Client.Packets.TeleportPacket
            {
                Scene = scene,
                Position = position
            }, senderId);
        }

        internal static void SendTeleportRequest(ushort targetPlayerId, ushort senderId)
        {
            var data = new Client.Packets.TeleportRequestPacket
            {
                PlayerId = senderId
            };

            sender.SendSingleData(PacketIDs.TeleportRequest, data, targetPlayerId);
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

            sender.SendSingleData(PacketIDs.Message, data, recipientId);
        }
    }
}

using SSMP.Networking.Packet;
using SSMP.Networking.Packet.Data;
using SSMPUtils.Client.Packets;
using SSMPUtils.Server.Modules;
using SSMPUtils.Utils;
using System;

namespace SSMPUtils.Server.Packets
{
    internal class PlayerHealthPacket : HealthPacket
    {
        public ushort PlayerId;

        public override void WriteData(IPacket packet)
        {
            base.WriteData(packet);
            packet.Write(PlayerId);
        }

        public override void ReadData(IPacket packet)
        {
            base.ReadData(packet);
            PlayerId = packet.ReadUShort();
        }
    }

    internal class SettingsPacket : Packet
    {
        public ServerSettings ServerSettings = null!;

        public override void WriteData(IPacket packet)
        {
            packet.Write(ServerSettings.HuddleEnabled);
            packet.Write(ServerSettings.TeleportsEnabled);
            packet.Write(ServerSettings.TeleportsNeedRequests);
            packet.Write(ServerSettings.DeathMessagesEnabled);
            packet.Write(ServerSettings.HealthbarsEnabled);
            packet.Write(ServerSettings.SpectateEnabled);
            packet.Write(ServerSettings.FreecamEnabled);
        }

        public override void ReadData(IPacket packet)
        {
            ServerSettings = new(false)
            {
                HuddleEnabled = packet.ReadBool(),
                TeleportsEnabled = packet.ReadBool(),
                TeleportsNeedRequests = packet.ReadBool(),
                DeathMessagesEnabled = packet.ReadBool(),
                HealthbarsEnabled = packet.ReadBool(),
                SpectateEnabled = packet.ReadBool(),
                FreecamEnabled = packet.ReadBool(),
            };
        }
    }
    public static class Packets
    {
        internal static IPacketData Instantiate(PacketIDs packetID)
        {
            return packetID switch
            {
                PacketIDs.Huddle => new TeleportPacket(),
                PacketIDs.TeleportRequest => new PacketDataCollection<TeleportRequestPacket>(),
                PacketIDs.TeleportAccept => new TeleportPacket(),
                PacketIDs.Message => new PacketDataCollection<MessagePacket>(),
                PacketIDs.PlayerHealth => new PacketDataCollection<PlayerHealthPacket>(),
                PacketIDs.Settings => new SettingsPacket(),
                _ => new ErrorThrowingPacket(packetID, true)
            };
        }

        internal class ErrorThrowingPacket : Packet
        {
            public ErrorThrowingPacket(PacketIDs id, bool server)
            {
                Log.LogError(id.ToString(), server);
                throw new NotImplementedException(id.ToString());
            }
        }
    }
}

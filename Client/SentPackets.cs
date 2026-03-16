using SSMP.Networking.Packet;
using SSMPUtils.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using SSMP.Math;

namespace SSMPUtils.Client.Packets
{
    internal class TeleportPacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;
        
        public ushort PlayerId;
        public string Scene = "";
        public Vector2 Position = Vector2.Zero;

        public void WriteData(IPacket packet)
        {
            packet.Write(PlayerId);
            packet.Write(Scene);
            packet.Write(Position);
        }

        public void ReadData(IPacket packet)
        {
            PlayerId = packet.ReadUShort();
            Scene = packet.ReadString();
            Position = packet.ReadVector2();
        }
    }

    internal class MessagePacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;

        public ushort PlayerId;

        public Messages Message;

        public void WriteData(IPacket packet)
        {
            packet.Write(PlayerId);
            packet.Write((ushort)Message);
        }

        public void ReadData(IPacket packet)
        {
            PlayerId = packet.ReadUShort();
            Message = (Messages)packet.ReadUShort();
        }
    }

    internal class TeleportRequestPacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;

        public ushort PlayerId;
        public void WriteData(IPacket packet)
        {
            packet.Write(PlayerId);
        }

        public void ReadData(IPacket packet)
        {
            PlayerId = packet.ReadUShort();
        }
    }

    public static class Packets
    {
        internal static IPacketData Instantiate(PacketIDs packetID)
        {
            return packetID switch
            {
                PacketIDs.Huddle => new TeleportPacket(),
                PacketIDs.TeleportRequest => new TeleportRequestPacket(),
                PacketIDs.TeleportAccept => new TeleportPacket(),
                PacketIDs.Message => new MessagePacket(),
                _ => throw new NotImplementedException()
            };
        }
    }
}

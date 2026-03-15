using SSMP.Networking.Packet;
using SSMP_Utils.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMP_Utils.Client.Packets
{
    internal class HuddlePacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;

        public string scene = "";
        public string gate = "";
        public Vector2 location;

        public void WriteData(IPacket packet)
        {
            packet.Write(scene);
            packet.Write(gate);
            packet.Write(location.x);
            packet.Write(location.y);
        }

        public void ReadData(IPacket packet)
        {
            scene = packet.ReadString();
            gate = packet.ReadString();
            location = new Vector2(packet.ReadFloat(), packet.ReadFloat());
        }
    }

    public static class Packets
    {
        internal static IPacketData Instantiate(PacketIDs packetID)
        {
            switch (packetID)
            {
                case PacketIDs.Huddle:
                    return new HuddlePacket();
                default:
                    throw new NotImplementedException(packetID.ToString());
            }
        }
    }
}

using SSMP.Networking.Packet;
using SSMP_Utils.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMP_Utils.Server.Packets
{
    internal class WarpPacket : Client.Packets.HuddlePacket { }

    public static class Packets
    {
        internal static IPacketData Instantiate(PacketIDs packetID)
        {
            switch (packetID)
            {
                case PacketIDs.Warp:
                    return new WarpPacket();
                default:
                    throw new NotImplementedException(packetID.ToString());
            }
        }
    }
}

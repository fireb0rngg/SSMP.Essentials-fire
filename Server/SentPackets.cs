using SSMP.Networking.Packet;
using SSMPUtils.Client.Packets;
using SSMPUtils.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMPUtils.Server.Packets
{
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

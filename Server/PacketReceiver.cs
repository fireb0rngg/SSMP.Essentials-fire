using SSMP.Api.Server;
using SSMP.Api.Server.Networking;
using SSMP_Utils.Utils;
using SSMP_Utils.Client.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSMP_Utils.Server
{
    internal static class PacketReceiver
    {
        static IServerAddonNetworkReceiver<PacketIDs> receiver;

        public static void Init()
        {
            receiver = Server.api.NetServer.GetNetworkReceiver<PacketIDs>(Server.instance, Client.Packets.Packets.Instantiate);
            receiver.RegisterPacketHandler<HuddlePacket>(PacketIDs.Huddle, OnHuddle);
        }

        public static void OnHuddle(ushort id, HuddlePacket data)
        {
            PacketSender.BroadcastWarp(data.scene, data.location, id);
        }
    }
}

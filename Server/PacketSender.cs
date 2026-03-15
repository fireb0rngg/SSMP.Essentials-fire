using SSMP.Api.Server;
using SSMP.Api.Server.Networking;
using SSMP.Networking.Packet;
using SSMP_Utils.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMP_Utils.Server
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

        internal static void BroadcastWarp(string scene, Vector2 location, ushort senderId)
        {
            Broadcast(PacketIDs.Warp, new Client.Packets.HuddlePacket
            {
                scene = scene,
                location = location
            }, senderId);
        }
    }
}

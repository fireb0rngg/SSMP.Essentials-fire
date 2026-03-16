using SSMP.Api.Client;
using SSMP.Api.Client.Networking;
using SSMPUtils.Client.Modules;
using SSMPUtils.Client.Packets;
using SSMPUtils.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector2 = SSMP.Math.Vector2;

namespace SSMPUtils.Client
{
    internal static class PacketSender
    {
        static IClientAddonNetworkSender<PacketIDs> sender;
        internal static void Init()
        {
            sender = Client.api.NetClient.GetNetworkSender<PacketIDs>(Client.instance);
        }

        internal static void SendMessage(ushort id, Messages message)
        {
            var data = new MessagePacket
            {
                Message = message,
                PlayerId = id,
            };
            sender.SendSingleData(PacketIDs.Message, data);
        }

        internal static void SendHuddle()
        {
            var foundScenePosition = Warp.GetHornetScenePosition(out var scene, out var position);
            if (!foundScenePosition)
            {
                return;
            }

            Log.LogDebug($"Sending huddle packet for scene {scene} at location {position}");
            sender.SendSingleData(PacketIDs.Huddle, new TeleportPacket
            {
                Scene = scene,
                Position = position,
            });
        }

        internal static void SendHuddle(ushort id)
        {
            if (GameManager.SilentInstance == null || GameManager.SilentInstance.GameState != GlobalEnums.GameState.PLAYING)
            {
                Client.LocalChat("Resume the game first!");
                return;
            }

            Log.LogDebug($"Sending huddle packet for user {id}");
            sender.SendSingleData(PacketIDs.Huddle, new TeleportPacket
            {
                Scene = "",
                Position = Vector2.Zero,
                PlayerId = id
            });
        }

        internal static void SendTeleportAccept(TeleportRequests.Request request)
        {
            var foundScenePosition = Warp.GetHornetScenePosition(out var scene, out var position);
            if (!foundScenePosition)
            {
                return;
            }

            request.Responded = true;

            Log.LogDebug($"Sending teleport accept to {request.PlayerId}");
            sender.SendSingleData(PacketIDs.TeleportAccept, new TeleportPacket
            {
                Scene = scene,
                Position = position,
                PlayerId = request.PlayerId
            });
        }
    }
}

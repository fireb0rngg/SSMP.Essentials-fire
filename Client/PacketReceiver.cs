using SSMP.Api.Client.Networking;
using SSMP_Utils.Utils;
using SSMP_Utils.Server.Packets;

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;

namespace SSMP_Utils.Client
{
    internal static class PacketReceiver
    {
        static IClientAddonNetworkReceiver<PacketIDs> receiver;

        public static void Init()
        {
            receiver = Client.api.NetClient.GetNetworkReceiver<PacketIDs>(Client.instance, Server.Packets.Packets.Instantiate);
            receiver.RegisterPacketHandler<WarpPacket>(PacketIDs.Warp, OnWarp);
        }

        internal static void OnWarp(WarpPacket data)
        {
            var scene = data.scene;
            var currentScene = SceneManager.GetActiveScene().name;
            var hornet = Common.HornetObject;
            
            if (hornet == null)
            {
                Client.LocalChat($"I couldn't warp you. Please join the rest of the server in {scene}.");
                return;
            }

            void SetHornetPosition()
            {
                GameManager.UnsafeInstance.OnFinishedEnteringScene -= SetHornetPosition;
                Log.LogInfo("Setting hornet position");
                //SSMP_UtilsPlugin.instance.StartCoroutine(Common.SetHornetPosition(data.location));
                //GameManager.instance.FinishedEnteringScene();
                hornet.transform.SetPosition2D(data.location);
                //HeroController.instance.RelinquishControl();
                //GameManager.instance.HazardRespawn();

                //Log.LogInfo(string.Join(", ", SceneTeleportMap.GetTeleportMap().Keys));
                //HeroController.instance.RegainControl();

                return;
            }

            if (scene == currentScene)
            {
                SetHornetPosition();
                return;
            }

            SceneTeleportMap.GetTeleportMap().TryGetValue(data.scene, out var teleport);

            GameManager.SceneLoadInfo loadInfo = new()
            {
                SceneName = data.scene,
                EntryGateName = teleport.TransitionGates[0],
                //EntrySkip = true,
                PreventCameraFadeOut = true,
                WaitForSceneTransitionCameraFade = false,
                AlwaysUnloadUnusedAssets = true,
                IsFirstLevelForPlayer = false
            };

            GameManager.UnsafeInstance.BeginSceneTransition(loadInfo);
            GameManager.UnsafeInstance.OnFinishedEnteringScene += SetHornetPosition;
        }
    }
}

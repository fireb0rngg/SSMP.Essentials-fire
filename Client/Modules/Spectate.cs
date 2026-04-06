using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SSMP.Api.Client;
using SSMPEssentials.Utils;
using SSMP.Game.Settings;

namespace SSMPEssentials.Client.Modules
{
    internal static class Spectate
    {
        internal enum MoveDir
        {
            Prev,
            Next
        }

        private static List<IClientPlayer> InScene = new();

        public static GameObject? FollowedPlayer;
        private static int FollowedPlayerIndex = -1;
        private static bool Following => FollowedPlayerIndex != -1;

        public static bool freecam = false;

        public static Vector2 FreecamMovementVector = Vector2.zero;
        private static GameObject? arrows;
        private static GameObject? LeftArrow;
        private static GameObject? RightArrow;
        private static GameObject? UpArrow;
        private static GameObject? DownArrow;

        public static void Init()
        {
            Client.api.ClientManager.PlayerEnterSceneEvent += AddPlayer;
            Client.api.ClientManager.PlayerLeaveSceneEvent += RemovePlayer;
            Client.api.ClientManager.PlayerDisconnectEvent += RemovePlayer;

            SceneManager.activeSceneChanged += (a, b) => ReturnToSelf(true);
            GameManager.instance.GamePausedChange += OnPauseChange;

            Client.OnServerSettingsUpdate += () =>
            {
                if (!Client.ServerSettings.FreecamEnabled && freecam) ReturnToSelf();
                if (!Client.ServerSettings.SpectateEnabled && Following) ReturnToSelf();

                InScene = GetPlayersInScene();
            };

            Client.OnSSMPSettingsUpdate += (s) =>
            {
                InScene = GetPlayersInScene(s);
            };
        }

        public static void Unload()
        {
            Client.api.ClientManager.PlayerEnterSceneEvent -= AddPlayer;
            Client.api.ClientManager.PlayerLeaveSceneEvent -= RemovePlayer;
        }

        public static void FocusOnPlayer(MoveDir dir)
        {
            if (InScene.Count == 0)
            {
                Client.LocalChat("There isn't anyone to spectate in this room.");
                return;
            }

            if (!Client.ServerSettings.SpectateEnabled)
            {
                Client.LocalChat("Spectating is currently disabled.");
                ReturnToSelf();
                return;
            }

            var prevIndex = FollowedPlayerIndex;
            EndPreviousMode();
            ToggleVignette(false);
            FollowedPlayerIndex = prevIndex;

            if (dir == MoveDir.Prev)
            {
                if (FollowedPlayerIndex <= 0) FollowedPlayerIndex = InScene.Count - 1;
                else FollowedPlayerIndex--;
            }
            else
            {
                if (FollowedPlayerIndex == -1 || FollowedPlayerIndex == InScene.Count - 1) FollowedPlayerIndex = 0;
                else FollowedPlayerIndex++;
            }

            var player = InScene[FollowedPlayerIndex];
            FollowedPlayer = player.PlayerObject;
            if (FollowedPlayer == null)
            {
                return;
            }

            var transform = FollowedPlayer.transform;
            ImmobilizePlayer();

            var target = GameManager.instance.cameraCtrl.camTarget;

            target.heroTransform = transform;
            FixMasks(FollowedPlayer);
            //target.SetDetachedFromHero(true);
            //GameManager.instance.cameraCtrl.mode = CameraController.CameraMode.LOCKED;
            //GameManager.instance.cameraCtrl.mode = CameraController.CameraMode.FOLLOWING;
            if (arrows != null) arrows.SetActive(true);
            ToggleUpDownArrows(false);
        }

        public static void Update()
        {
            var cam = GameManager.SilentInstance != null ? GameManager.SilentInstance.cameraCtrl : null;
            if (cam == null) return;

            if (freecam)
            {
                var camPosition = cam.cam.transform.position;
                var position = FreecamMovementVector + (Vector2)camPosition;
                SetFreecamBoundsArrows(position);
                cam.SnapTo(position.x, position.y);
            }
            else if (Following)
            {
                //var lockZones = cam.lockZoneList;
                //var validLockZones = lockZones.Where(z => z.insideGameObjects.Contains(FollowedPlayer)).ToList();
                //cam.lockZoneList = validLockZones;
                cam.camTarget.ExitLockZone();
                return;
                //var player = InScene[FollowedPlayerIndex];
                //position = player.PlayerObject.transform.position;
            }
            else
            {
                return;
            }

            //GameManager.instance.cameraCtrl.isGameplayScene = false;
        }

        private static void SetFreecamBoundsArrows(Vector2 position)
        {
            var camera = GameManager.instance.cameraCtrl;
            const float grace = 0.2f;
            var left = position.x <= Constants.CAM_BOUND_X + grace;
            var right = position.x >= camera.xLimit - grace;
            var down = position.y <= Constants.CAM_BOUND_Y + grace;
            var up = position.y >= camera.yLimit - grace;

            if (LeftArrow != null) LeftArrow.SetActive(!left);
            if (RightArrow != null) RightArrow.SetActive(!right);
            if (DownArrow != null) DownArrow.SetActive(!down);
            if (UpArrow != null) UpArrow.SetActive(!up);
        }

        private static void EndPreviousMode()
        {
            FreecamMovementVector = Vector2.zero;
            freecam = false;
            FollowedPlayerIndex = -1;
            FollowedPlayer = null;
            ToggleUpDownArrows(true);
            ToggleVignette(true);

            GameManager.instance.cameraCtrl.camTarget.heroTransform = HeroController.instance.transform;
            FixMasks(HeroController.instance.gameObject);
        }
        
        public static void ReturnToSelf(bool wasSceneChange = false)
        {

            FollowedPlayerIndex = -1;
            FollowedPlayer = null;
            FreecamMovementVector = Vector2.zero;
            freecam = false;
            ToggleVignette(true);
            if (HeroController.instance != null)
            {
                RestoreControl();
                //StartCoroutine(GameManager.instance.cameraCtrl.DoPositionToHero(true));
                GameManager.instance.cameraCtrl.camTarget.heroTransform = HeroController.instance.transform;

                if (!wasSceneChange)
                {
                    FixMasks(HeroController.instance.gameObject);
                }
            }

            if (arrows != null) arrows.SetActive(false);
        }

        private static void ToggleVignette(bool status)
        {
            var hornet = HeroController.SilentInstance != null ? HeroController.SilentInstance.gameObject : null;
            if (hornet != null)
            {
                var vignette = hornet.FindGameObjectInChildren("Vignette");
                if (vignette != null) vignette.SetActive(status);
            }
        }

        private static void FixMasks(GameObject playerObject)
        {
            var masks = Resources.FindObjectsOfTypeAll<Remasker>();
            var hero = playerObject.GetComponent<Collider2D>();
            foreach (var mask in masks)
            {
                if (mask == null) continue;

                var collider = mask.GetComponent<Collider2D>();
                if (collider != null && collider.IsTouching(hero))
                {
                    mask.Entered();
                }
                else
                {
                    mask.Exited(false);
                }
            }
        }

        public static void Freecam()
        {
            if (freecam) return;
            if (!Client.ServerSettings.FreecamEnabled)
            {
                Client.LocalChat("Freecam is currently disabled.");
                ReturnToSelf();
                return;
            }

            EndPreviousMode();
            ToggleVignette(false);

            freecam = true;
            ImmobilizePlayer();
            FixMasks(HeroController.instance.gameObject);
            if (arrows != null) arrows.SetActive(true);
            ToggleUpDownArrows(true);
        }

        private static void ImmobilizePlayer()
        {
            HeroController.instance.IgnoreInput();
            HeroController.instance.ResetMotion();
        }

        private static void RestoreControl()
        {
            HeroController.instance.UnPauseInput();
            //GameManager.instance.cameraCtrl.isGameplayScene = true;
        }

        public static void CreateFreecamUI()
        {
            var inGame = GameManager.instance.gameCams.hudCamera.transform.GetChild(0);
            var existingArrows = inGame.GetChild(4).GetChild(2).GetChild(1).GetChild(0).gameObject;

            var newArrows = GameObject.Instantiate(existingArrows, inGame);
            newArrows.SetActive(false);

            DownArrow = newArrows.transform.GetChild(0).gameObject;
            LeftArrow = newArrows.transform.GetChild(1).gameObject;
            RightArrow = newArrows.transform.GetChild(2).gameObject;
            UpArrow = newArrows.transform.GetChild(3).gameObject;

            DownArrow.transform.localPosition = new Vector3(0, -7.5f, 45);
            LeftArrow.transform.localPosition = new Vector3(-13.5f, 0, 45);
            RightArrow.transform.localPosition = new Vector3(13.5f, 0, 45);
            UpArrow.transform.localPosition = new Vector3(0, 7.5f, 45);

            arrows = newArrows;
        }

        private static void ToggleUpDownArrows(bool status)
        {
            if (arrows == null) return;
            if (DownArrow) DownArrow.SetActive(status);
            if (UpArrow) UpArrow.SetActive(status);
        }

        private static void OnPauseChange(bool isPaused)
        {
            if (isPaused) return;
            if (freecam || Following) ReturnToSelf();
        }

        private static bool CanSpectatePlayer(IClientPlayer player)
        {
            var settings = Client.GetClientServerSettings();
            return CanSpectatePlayer(player, settings);
        }

        private static bool CanSpectatePlayer(IClientPlayer player, ServerSettings settings)
        {
            if (!Client.ServerSettings.SpectateTeamOnly) return true;
            if (!settings.TeamsEnabled) return true;

            return player.Team == Client.api.ClientManager.Team;
        }

        public static void AddPlayer(IClientPlayer player)
        {
            if (!CanSpectatePlayer(player)) return;

            if (InScene.Contains(player)) return;

            InScene.Add(player);
        }

        public static void RemovePlayer(IClientPlayer player)
        {
            var index = InScene.FindIndex(p => p.Id == player.Id);
            if (index == -1) return;
                
            InScene.RemoveAt(index);

            if (Following)
            {
                if (InScene.Count == 0) ReturnToSelf();
                else if (index == InScene.Count - 1) FocusOnPlayer(MoveDir.Prev);
            }    
        }

        private static List<IClientPlayer> GetPlayersInScene()
        {
            var api = Client.api;
            var players = api.ClientManager.Players.ToList();

            return players.Where(p => p.IsInLocalScene && CanSpectatePlayer(p)).ToList();
        }

        private static List<IClientPlayer> GetPlayersInScene(ServerSettings settings)
        {
            var api = Client.api;
            var players = api.ClientManager.Players.ToList();

            return players.Where(p => p.IsInLocalScene && CanSpectatePlayer(p, settings)).ToList();
        }
    }
}

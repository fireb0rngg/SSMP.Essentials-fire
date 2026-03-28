using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SSMP.Api.Client;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Client.Modules
{
    internal static class Spectate
    {
        internal enum MoveDir
        {
            Prev,
            Next
        }

        static List<IClientPlayer> InScene = new();
        static int FollowedPlayerIndex = -1;
        public static GameObject? FollowedPlayer;
        static bool Following => FollowedPlayerIndex != -1;

        public static bool freecam = false;

        public static Vector2 FreecamMovementVector = Vector2.zero;
        static GameObject? arrows;
        static GameObject? LeftArrow;
        static GameObject? RightArrow;
        static GameObject? UpArrow;
        static GameObject? DownArrow;

        public static void Init()
        {
            Client.api.ClientManager.PlayerEnterSceneEvent += AddPlayer;
            Client.api.ClientManager.PlayerLeaveSceneEvent += RemovePlayer;
            SceneManager.activeSceneChanged += (a, b) => ReturnToSelf(true);
            GameManager.instance.GamePausedChange += OnPauseChange;

            Client.OnServerSettingsUpdate += () =>
            {
                if (!Client.ServerSettings.FreecamEnabled && freecam) ReturnToSelf();
                if (!Client.ServerSettings.SpectateEnabled && Following) ReturnToSelf();
            };
        }

        public static void Unload()
        {
            Client.api.ClientManager.PlayerEnterSceneEvent -= AddPlayer;
            Client.api.ClientManager.PlayerLeaveSceneEvent -= RemovePlayer;
        }

        public static void FocusOnPlayer(MoveDir dir)
        {
            if (InScene.Count == 0) return;
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

        static void SetFreecamBoundsArrows(Vector2 position)
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

        public static void EndPreviousMode()
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

        static void ToggleVignette(bool status)
        {
            var hornet = HeroController.SilentInstance != null ? HeroController.SilentInstance.gameObject : null;
            if (hornet != null)
            {
                var vignette = hornet.FindGameObjectInChildren("Vignette");
                if (vignette != null) vignette.SetActive(status);
            }
        }

        static void FixMasks(GameObject playerObject)
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

        static void ImmobilizePlayer()
        {
            HeroController.instance.IgnoreInput();
            HeroController.instance.ResetMotion();
        }

        static void RestoreControl()
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

            DownArrow.transform.localPosition = new Vector3(0, -7.5f, 45); // down
            LeftArrow.transform.localPosition = new Vector3(-13.5f, 0, 45); // left
            RightArrow.transform.localPosition = new Vector3(13.5f, 0, 45); // right
            UpArrow.transform.localPosition = new Vector3(0, 7.5f, 45); // up

            arrows = newArrows;
        }

        public static void ToggleUpDownArrows(bool status)
        {
            if (arrows == null) return;
            arrows.transform.GetChild(0).gameObject.SetActive(status); // down
            arrows.transform.GetChild(3).gameObject.SetActive(status); // up
        }

        private static void OnPauseChange(bool isPaused)
        {
            if (isPaused) return;
            if (freecam || Following) ReturnToSelf();
        }

        static void AddPlayer(IClientPlayer player)
        {
            var exists = InScene.Any(p => p.Id == player.Id);
            if (exists) return;

            InScene.Add(player);
        }

        static void RemovePlayer(IClientPlayer player)
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

        static List<IClientPlayer> GetPlayersInScene()
        {
            var api = Client.api;
            var players = api.ClientManager.Players.ToList();

            return players.Where(p => p.IsInLocalScene).ToList();
        }
    }
}

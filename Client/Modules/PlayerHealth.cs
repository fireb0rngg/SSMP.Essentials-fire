using System;
using System.Linq;
using GlobalEnums;
using UnityEngine;
using UnityEngine.UI;
using SSMP.Api.Client;
using SSMPEssentials.Data;
using SSMPEssentials.Utils;
using Object = UnityEngine.Object;

namespace SSMPEssentials.Client.Modules
{
    internal static class PlayerHealth
    {
        static GameObject? maskPrefab;
        public const int MAX_HEALTH = 18;
        public const int NORMAL_MASK_ID = 9;
        public const int MISSING_MASK_ID = 0;
        public const int BLUE_MASK_ID = 50;
        public const int HIVE_MASK_ID = 194;

        public static void OnHealthChange()
        {
            var hc = HeroController.instance.playerData;

            var data = new HealthData
            {
                Health = hc.health,
                MaxHealth = hc.maxHealth,
                BlueHealth = hc.healthBlue,
                LifebloodState = HeroController.instance.IsInLifebloodState
            };
            
            PacketSender.SendHealth(data);
        }

        public static void BlueHealthAddListener()
        {
            var healthPath = "In-game/Anchor TL/Hud Canvas Offset/Hud Canvas/Health";
            var localHealth = GameCameras.instance.hudCamera.gameObject.FindGameObjectInChildren(healthPath);
            if (localHealth != null)
            {
                var register = localHealth.GetComponents<EventRegister>().FirstOrDefault(r => r.SubscribedEvent == "ADD BLUE HEALTH");
                if (register != null)
                {
                    register.ReceivedEvent += OnHealthChange;
                }
            }
        }

        public static void OnPlayerEnter(IClientPlayer player)
        {
            var container = player.PlayerContainer;
            if (container == null) return;

            var healthDisplay = FindOrCreateHealthBar(container, player.Id);
            healthDisplay.Refresh();
        }

        public static void SetPlayerHealth(IClientPlayer player, HealthData healthData)
        {
            var clampedHealth = Math.Clamp(healthData.Health, 0, 10);
            var clampedMax = Math.Clamp(healthData.MaxHealth, 0, 10);
            var clampedBlue = Math.Clamp(healthData.BlueHealth, 0, 8);

            var health = PlayerDataTracker.ClientInstance.GetPlayer(player.Id).Health;
            health.Health = clampedHealth;
            health.MaxHealth = clampedMax;
            health.BlueHealth = clampedBlue;
            health.LifebloodState = healthData.LifebloodState;

            var playerContainer = player.PlayerContainer;

            if (playerContainer == null) return;

            var healthDisplay = FindOrCreateHealthBar(playerContainer, player.Id);
            healthDisplay.Refresh();
        }

        public static HealthDisplay FindOrCreateHealthBar(GameObject playerContainer, ushort id)
        {
            var display = playerContainer.GetComponentInChildren<HealthDisplay>();
            if (display == null)
            {
                Log.LogDebug("Creating display");
                var displayGameObject = new GameObject("Player Health Display");
                displayGameObject.transform.SetParentReset(playerContainer.transform);

                display = displayGameObject.AddComponent<HealthDisplay>();
            }
            else
            {
                Log.LogDebug("Display found");
            }
            display.Owner = id;
            return display;
        }

        static GameObject GetMask()
        {
            if (maskPrefab != null) return maskPrefab;

            var path = "In-game/Anchor TL/Hud Canvas Offset/Hud Canvas/Health/Health 2+/Idle";
            var localMask = GameCameras.instance.hudCamera.gameObject.FindGameObjectInChildren(path);
            if (localMask == null)
            {
                throw new Exception("Couldn't find health icon");
            }
            
            var mask = Object.Instantiate(localMask);

            if (mask.TryGetComponent<SpriteFlash>(out var flash))
            {
                Object.DestroyImmediate(flash);
            }

            if (mask.TryGetComponent<SteelSoulAnimProxy>(out var proxy))
            {
                Object.DestroyImmediate(proxy);
            }

            mask.layer = (int)PhysLayers.DEFAULT;
            mask.SetActive(false);
            maskPrefab = mask;
            return maskPrefab;
        }

        public static void CreateHealthBar(GameObject healthBar)
        {
            healthBar.layer = (int)PhysLayers.UI;
            var spacing = new Vector2(0, 0.64f);
            var canvasScale = 0.222f;
            var position = 2.3f;
            var columns = 6;
            var cellSize = new Vector2(2.06f, 1.55f);
            var maskScale = new Vector2(1.43f, 1.43f);
            var canvasWidth = 15;
            var canvasHeight = 9;
            
            healthBar.AddComponent<KeepWorldScalePositive>();
            var canvas = healthBar.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            //canvas.sortingLayerName = "HUD";


            var group = healthBar.AddComponent<GridLayoutGroup>();
            group.childAlignment = TextAnchor.LowerCenter;
            group.startCorner = GridLayoutGroup.Corner.UpperLeft;
            group.startAxis = GridLayoutGroup.Axis.Horizontal;
            group.cellSize = cellSize;
            group.spacing = spacing;
            group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            group.constraintCount = columns;

            healthBar.AddComponent<ContentSizeFitter>();

            var transform = healthBar.GetComponent<RectTransform>();
            transform.pivot = new Vector2(0.5f, 0);
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasWidth);
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasHeight);
            transform.SetLocalPositionY(position);
            transform.SetScale2D(new Vector2(canvasScale, canvasScale));

            var prefab = GetMask();
            for (int i = 0; i < MAX_HEALTH; i++)
            {
                var newObj = Object.Instantiate(prefab);
                newObj.transform.SetParentReset(healthBar.transform);

                var rect = newObj.AddComponentIfNotPresent<RectTransform>();
                rect.SetScale2D(maskScale);

                //var renderer = newObj.GetComponent<tk2dSprite>();
                //renderer.SetSprite(NORMAL_MASK_ID);

                if (newObj.TryGetComponent<tk2dSpriteAnimator>(out var animator))
                {
                    Object.Destroy(animator);
                }
            }
        }
    }

    public class HealthDisplay : MonoBehaviour
    {
        public ushort Owner;
        public HealthData Health = new();

        void Awake()
        {
            Log.LogDebug("Health display awoke");
            transform.Reset();
            PlayerHealth.CreateHealthBar(gameObject);
            Client.OnServerSettingsUpdate += OnSettingsChange;
            OnSettingsChange();
        }

        void OnSettingsChange()
        {
            //if (!gameObject.transform.parent.gameObject.activeInHierarchy) return;

            if (Client.ServerSettings.HealthbarsEnabled)
            {
                Log.LogDebug("Settings changed, enabling");
                gameObject.SetActive(true);
                Refresh();
            }
            else
            {
                Log.LogDebug("Settings changed, disabling");
                gameObject.SetActive(false);
            }
        }

        public void Refresh()
        {
            var data = PlayerDataTracker.ClientInstance.GetPlayer(Owner);
            Health = data.Health;

            var totalHealth = Health.MaxHealth + Health.BlueHealth;

            for (int i = 0; i < PlayerHealth.MAX_HEALTH; i++)
            {
                var child = transform.GetChild(i);
                var healthNum = i + 1;
                if (healthNum > totalHealth)
                {
                    child.gameObject.SetActive(false);
                    continue;
                }
                
                child.gameObject.SetActive(true);

                var renderer = child.GetComponent<tk2dSprite>();

                if (healthNum <= Health.Health)
                {
                    if (Health.LifebloodState) renderer.SetSprite(PlayerHealth.BLUE_MASK_ID);
                    else renderer.SetSprite(PlayerHealth.NORMAL_MASK_ID);
                }
                else if (healthNum <= Health.MaxHealth) renderer.SetSprite(PlayerHealth.MISSING_MASK_ID);
                else renderer.SetSprite(PlayerHealth.BLUE_MASK_ID);
            }

            gameObject.SetActive(Client.ServerSettings.HealthbarsEnabled);
        }
    }
}

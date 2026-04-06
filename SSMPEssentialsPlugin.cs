using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using SSMPEssentials.Client.Modules.Patches;
using SSMPEssentials.Client.Modules;
using SSMPEssentials.Utils.Tests;
using SSMPEssentials.Utils;

namespace SSMPEssentials;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.bobbythecatfish.ssmp.essentials")]
[BepInDependency("ssmp", BepInDependency.DependencyFlags.HardDependency)]
public partial class SSMPEssentialsPlugin : BaseUnityPlugin
{
    TestManager testManager = new TestManager();
    public static readonly List<Action> NextFrames = [];
    List<Action> _nextFrames = [];

    internal static SSMPEssentialsPlugin instance;
    private void Awake()
    {
        instance = this;
        // Put your initialization logic here


        SSMP.Api.Client.ClientAddon.RegisterAddon(new Client.Client());
        SSMP.Api.Server.ServerAddon.RegisterAddon(new Server.Server());

        Utils.Config.Init(Config);
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");

        var harmony = new Harmony("ssmp.essentials");
        harmony.PatchAll();
        //Harmony.CreateAndPatchAll(typeof(MaskerPatch), "ssmp.essentials");
        //Harmony.CreateAndPatchAll(typeof(DamageHeroPatch), "ssmp.essentials");
        //Harmony.CreateAndPatchAll(typeof(HealthPatch), "ssmp.essentials");


        HeroController.OnHeroInstanceSet += InitializeHCModules;

    }

    private void InitializeHCModules(HeroController controller)
    {
        HeroController.OnHeroInstanceSet -= InitializeHCModules;

        Spectate.CreateFreecamUI();
        PlayerDeaths.Init();
        controller.OnDeath += PlayerDeaths.OnDeath;
        PlayerHealth.BlueHealthAddListener();
    }

    private void Update()
    {
        if (Utils.Config.TestsEnabled)
        {
            testManager.Update();
        }
        if (_nextFrames.Count > 0)
        {
            var actions = _nextFrames.ToArray();
            _nextFrames.Clear();
            foreach (var action in actions)
            {
                action.Invoke();
            }
        }

        Inputs.Update();
    }

    private void LateUpdate()
    {
        if (NextFrames.Count > 0)
        {
            _nextFrames = [.. NextFrames];
            NextFrames.Clear();
        }

        Spectate.Update();
    }
}

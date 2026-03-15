using BepInEx;
using SSMP_Utils.Client.Modules;
using SSMP_Utils.Utils;
using SSMP_Utils.Utils.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SSMP_Utils.Client.Modules.Patches;

namespace SSMP_Utils;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.bobbythecatfish.ssmp.utils")]
public partial class SSMP_UtilsPlugin : BaseUnityPlugin
{
    TestManager testManager = new TestManager();
    public static readonly List<Action> NextFrames = [];
    List<Action> _nextFrames = [];

    internal static SSMP_UtilsPlugin instance;
    private void Awake()
    {
        instance = this;
        // Put your initialization logic here
        Log.SetLogger(Logger);
        Utils.Config.Init(Config);
        Log.LogInfo($"Plugin {Name} ({Id}) has loaded!");

        SSMP.Api.Client.ClientAddon.RegisterAddon(new Client.Client());
        SSMP.Api.Server.ServerAddon.RegisterAddon(new Server.Server());

        Harmony.CreateAndPatchAll(typeof(MaskerPatch), "ssmp.utils");

        HeroController.OnHeroInstanceSet += Spectate.CreateFreecamUI;

    }

    private void Update()
    {
        if (Utils.Config.TestsEnabled)
        {
            testManager.Update();
        }
        if (_nextFrames.Count > 0)
        {
            foreach (var action in _nextFrames)
            {
                Log.LogInfo("Executing next frame action");
                action.Invoke();
            }
            _nextFrames.Clear();
        }

        Inputs.Update();
    }

    private void LateUpdate()
    {
        if (NextFrames.Count > 0)
        {
            Log.LogInfo($"Adding {NextFrames.Count} next frame actions");
            _nextFrames = [.. NextFrames];
            NextFrames.Clear();
        }

        Spectate.Update();
    }
}

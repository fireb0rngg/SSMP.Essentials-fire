using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMPUtils.Utils
{
    internal static class Config
    {
        public static uint SSMPApiVersion = 1;

        static ConfigEntry<bool> _testsEnabled;
        public static bool TestsEnabled => _testsEnabled?.Value ?? false;

        static ConfigEntry<KeyCode> _spectateNext;
        public static KeyCode SpectateNext => _spectateNext.Value;

        static ConfigEntry<KeyCode> _spectatePrevious;
        public static KeyCode SpectatePrevious => _spectatePrevious.Value;

        static ConfigEntry<KeyCode> _exitSpectate;
        public static KeyCode ExitSpectate => _exitSpectate.Value;

        static ConfigEntry<KeyCode> _freecamToggle;
        public static KeyCode FreecamToggle => _freecamToggle.Value;

        public static void Init(ConfigFile config)
        {
#if DEBUG
            _testsEnabled = config.Bind("Misc", "Run Tests", false);
#endif
            _spectatePrevious = config.Bind("Keybinds", "Spectate Previous Player", KeyCode.Alpha2);
            _spectateNext = config.Bind("Keybinds", "Spectate Next Player", KeyCode.Alpha3);
            _exitSpectate = config.Bind("Keybinds", "Exit Spectate Mode", KeyCode.Alpha4);
            _freecamToggle = config.Bind("Keybinds", "Toggle Freecam Mode", KeyCode.Alpha5);
        }
    }
}

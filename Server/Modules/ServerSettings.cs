using Newtonsoft.Json;
using SSMPUtils.Utils;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SSMPUtils.Server.Modules
{
    internal class ServerSettings
    {
        public bool HuddleEnabled = true;
        public bool TeleportsEnabled = true;
        public bool TeleportsNeedRequests = true;
        public bool DeathMessagesEnabled = true;
        public bool HealthbarsEnabled = true;
        public bool SpectateEnabled = true;
        public bool FreecamEnabled = true;

        private static string Filepath()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(dir, "ServerSettings.json");
        }

        public void ReadFromFile()
        {
            var path = Filepath();
            if (!File.Exists(path))
            {
                Log.LogWarning($"{path} doesn't exist");
                return;
            }

            var file = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, bool>>(file);
            if (settings == null) return;

            void SetSetting(string key, ref bool value)
            {
                if (settings.TryGetValue(key, out var val))
                {
                    value = val;
                }
            }

            SetSetting("HuddleEnabled", ref HuddleEnabled);
            SetSetting("TeleportsEnabled", ref TeleportsEnabled);
            SetSetting("TeleportsNeedRequests", ref TeleportsNeedRequests);
            SetSetting("DeathMessages", ref DeathMessagesEnabled);
            SetSetting("Healthbars", ref HealthbarsEnabled);
            SetSetting("SpectateEnabled", ref SpectateEnabled);
            SetSetting("FreecamEnabled", ref FreecamEnabled);
        }

        public void OnUpdate()
        {
            var dictForm = new Dictionary<string, bool>
            {
                {"HuddleEnabled", HuddleEnabled},
                {"TeleportsEnabled", TeleportsEnabled },
                {"TeleportsNeedRequests", TeleportsNeedRequests},
                {"DeathMessages", DeathMessagesEnabled},
                {"Healthbars", HealthbarsEnabled},
                {"SpectateEnabled", SpectateEnabled},
                {"FreecamEnabled", FreecamEnabled },
            };

            var settings = JsonConvert.SerializeObject(dictForm, Formatting.Indented);
            if (settings == null) return;

            var path = Filepath();
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Log.LogWarning($"{path} directory doesn't exist");
                return;
            }
            File.WriteAllText(path, settings);

            PacketSender.BroadcastSettingsUpdate();
        }

        public ServerSettings(bool client)
        {
            HuddleEnabled = !client;
            TeleportsEnabled = !client;
            TeleportsNeedRequests = !client;
            DeathMessagesEnabled = !client;
            HealthbarsEnabled = !client;
            SpectateEnabled = !client;
            FreecamEnabled = !client;
        }
    }
}

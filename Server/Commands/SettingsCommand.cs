using System.Collections.Generic;
using System.Reflection;
using SSMP.Api.Command.Server;
using SSMP.Game.Settings;

namespace SSMPEssentials.Server.Commands
{
    internal class SettingsCommand : IServerCommand
    {
        public bool AuthorizedOnly => true;
        public string Trigger => "/essentials";
        public string[] Aliases => ["/essential", "/ess", "/e"];

        const string SETTING_SUB = "set";

        public void Execute(ICommandSender sender, string[] arguments)
        {
            // Invalid syntax
            var syntax = $"Invalid Syntax. {Trigger} {SETTING_SUB} <setting> <true/false>";
            if (arguments.Length < 2)
            {
                sender.SendMessage(syntax);
                return;
            }

            // Get settings
            var settingProps = Server.ServerSettings.GetProps();
            var validSettingNames = new List<string>();
            var validSettings = new Dictionary<string, PropertyInfo>();
            foreach (var prop in settingProps)
            {
                var aliasAttribute = prop.GetCustomAttribute<SettingAliasAttribute>();
                validSettings.Add(prop.Name, prop);
                if (aliasAttribute == null || aliasAttribute.Aliases.Length == 0)
                {
                    validSettingNames.Add(prop.Name);
                    continue;
                }

                // Prefer first alias for string name
                validSettingNames.Add(aliasAttribute.Aliases[0]);
                foreach (var alias in aliasAttribute.Aliases)
                {
                    validSettings.Add(alias, prop);
                }
            }

            // No setting provided, send list of settings
            var validSettingsStr = $"Valid settings are {string.Join(", ", validSettingNames)}";
            if (arguments.Length == 2)
            {
                sender.SendMessage(validSettingsStr);
                return;
            }

            // Setting not found, send list of settings
            var settingNameInput = arguments[2].ToLower().Replace("_", "");
            if (!validSettings.TryGetValue(settingNameInput, out var setting))
            {
                sender.SendMessage(validSettingsStr);
                return;
            }

            // Get current setting value if not provided
            if (arguments.Length == 3)
            {
                var currentValue = setting.GetValue(Server.ServerSettings);
                sender.SendMessage($"Setting '{setting.Name}' currently has value: {currentValue}");
                return;
            }

            // Invalid value
            var valueStr = arguments[3].ToLower();
            if (valueStr != "true" && valueStr != "false")
            {
                sender.SendMessage(syntax);
                return;
            }
            
            // Update value
            var value = valueStr == "true";
            var status = value ? "enabled" : "disabled";

            setting.SetValue(Server.ServerSettings, value);
            Server.BroadcastMessage($"Changed setting '{setting.Name}' to: {value}");

            Server.ServerSettings.WriteToFile();
            PacketSender.BroadcastSettingsUpdate();
        }

    }
}

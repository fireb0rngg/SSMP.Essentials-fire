using SSMP.Api.Command.Server;
using System.Linq;

namespace SSMPUtils.Server.Commands
{
    internal class SettingsCommand : IServerCommand
    {
        public bool AuthorizedOnly => true;
        public string Trigger => "/essentials";
        public string[] Aliases => ["/essential", "/ess", "/e"];

        const string SETTING_SUB = "set";
        const string HUDDLE_SETTING = "huddle";
        const string TELEPORT_SETTING = "tp";
        const string TELEPORT_REQUEST_SETTING = "tprequests";
        const string DEATH_MESSAGES_SETTING = "deathmessages";
        const string HEALTHBAR_SETTING = "healthbars";
        const string SPECTATE_SETTING = "spectate";
        const string FREECAM_SETTING = "freecam";

        public void Execute(ICommandSender sender, string[] arguments)
        {
            var syntax = $"Invalid Syntax. {Trigger} {SETTING_SUB} <setting> <true/false>";
            if (arguments.Length < 2)
            {
                sender.SendMessage(syntax);
                return;
            }

            var validSettings = $"Valid settings are {HUDDLE_SETTING}, {TELEPORT_SETTING}, {TELEPORT_REQUEST_SETTING}, {DEATH_MESSAGES_SETTING}, {HEALTHBAR_SETTING}, {SPECTATE_SETTING}, {FREECAM_SETTING}";
            if (arguments.Length == 2)
            {
                sender.SendMessage(validSettings);
                return;
            }

            if (arguments.Length != 4)
            {
                sender.SendMessage(syntax);
                return;
            }

            var valueStr = arguments[3].ToLower();
            if (valueStr != "true" && valueStr != "false")
            {
                sender.SendMessage(syntax);
                return;
            }

            var setting = arguments[2];
            var value = valueStr == "true";
            var status = value ? "enabled" : "disabled";

            switch (setting)
            {
                case HUDDLE_SETTING:
                    Server.ServerSettings.HuddleEnabled = value;
                    Server.BroadcastMessage($"Huddles are now {status}");
                    break;
                case TELEPORT_SETTING:
                    Server.ServerSettings.TeleportsEnabled = value;
                    Server.BroadcastMessage($"Teleporting is now {status}");
                    break;
                case TELEPORT_REQUEST_SETTING:
                    Server.ServerSettings.TeleportsNeedRequests = value;
                    if (Server.ServerSettings.TeleportsEnabled)
                    {
                        status = value ? "now" : "no longer";
                        Server.BroadcastMessage($"Teleports {status} require requests");
                    }
                    break;
                case DEATH_MESSAGES_SETTING:
                    Server.ServerSettings.DeathMessagesEnabled = value;
                    Server.BroadcastMessage($"Death messages are now {status}");
                    break;
                case HEALTHBAR_SETTING:
                    Server.ServerSettings.HealthbarsEnabled = value;
                    Server.BroadcastMessage($"Healthbars are now {status}");
                    break;
                case SPECTATE_SETTING:
                    Server.ServerSettings.SpectateEnabled = value;
                    Server.BroadcastMessage($"Spectating is now {status}");
                    break;
                case FREECAM_SETTING:
                    Server.ServerSettings.FreecamEnabled = value;
                    Server.BroadcastMessage($"Freecam is now {status}");
                    break;
                default:
                    sender.SendMessage(validSettings);
                    return;
            }

            Server.ServerSettings.OnUpdate();
        }

    }
}

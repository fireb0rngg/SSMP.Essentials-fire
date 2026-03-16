using SSMP.Api.Command.Client;
using SSMP.Api.Command.Server;
using SSMPUtils.Client.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMPUtils.Client.Commands
{
    internal class Huddle : IClientCommand
    {
        public bool AuthorizedOnly => false;
        public string Trigger => "/huddle";
        public string[] Aliases => ["/tpall"];
        public void Execute(string[] arguments)
        {
            // Huddle to a specific user
            if (arguments.Length > 1)
            {
                var args = arguments.ToList();
                args.RemoveAt(0);

                // Username wasn't blank
                var username = string.Join(" ", args);
                if (username.Length > 0)
                {
                    // Attempt to find player
                    var player = Client.GetPlayerByName(username);
                    if (player != null)
                    {
                        PacketSender.SendHuddle(player.Id);
                        return;
                    }

                    // Attempt failed
                    Client.LocalChat($"Player '{username}' not found.");
                    return;
                }
            }

            // Base case, no username
            PacketSender.SendHuddle();
        }
    }
}

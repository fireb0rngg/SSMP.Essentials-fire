using System.Linq;
using UnityEngine;
using SSMP.Api.Client;
using SSMP.Api.Command.Client;
using SSMPEssentials.Client.Modules;

namespace SSMPEssentials.Client.Commands
{
    internal class TeleportAccept : IClientCommand
    {
        public string Trigger => "/tpaccept";

        public string[] Aliases => ["/tpa"];

        public void Execute(string[] arguments)
        {
            var player = TeleportRequest.GetUsernameFromArgs(arguments, false);
            TeleportRequests.RespondToRequest(true, player);
        }
    }

    internal class TeleportDeny : IClientCommand
    {
        public string Trigger => "/tpdeny";

        public string[] Aliases => ["/tpd"];

        public void Execute(string[] arguments)
        {
            var player = TeleportRequest.GetUsernameFromArgs(arguments, false);
            TeleportRequests.RespondToRequest(false, player);
        }
    }

    internal class TeleportBack : IClientCommand
    {
        public string Trigger => "/back";
        public string[] Aliases => [];
        public static string PreviousScene = "";
        public static Vector2 PreviousLocation = Vector2.zero;
        public void Execute(string[] arguments)
        {
            if (!Client.ServerSettings.BackEnabled)
            {
                Client.LocalChat("/back is currently disabled.");
                return;
            }

            if (PreviousScene == "")
            {
                Client.LocalChat("I don't have your previous location.");
                return;
            }

            var warp = new Warp(PreviousScene, PreviousLocation);
            warp.WarpToPosition();
        }
    }

    internal class TeleportRequest : IClientCommand
    {
        public string Trigger => "/tp";

        public string[] Aliases => ["/teleport"];

        public void Execute(string[] arguments)
        {
            var player = GetUserFromArgs(arguments, true);
            if (player == null) return;

            PacketSender.SendTeleportRequest(player.Id);
        }

        public static string GetUsernameFromArgs(string[] arguments, bool required)
        {
            if (arguments.Length < 2)
            {
                if (required) Client.LocalChat("You need to specify a user.");
                return "";
            }

            var args = arguments.ToList();
            args.RemoveAt(0);

            // Username wasn't blank
            var username = string.Join(" ", args);

            if (string.IsNullOrEmpty(username))
            {
                if (required) Client.LocalChat("You need to specify a user.");
                return "";
            }

            return username;
        }

        public static IClientPlayer? GetUserFromArgs(string[] arguments, bool required)
        {
            var username = GetUsernameFromArgs(arguments, required);
            if (username == "") return null;

            // Attempt to find player
            var player = Client.GetPlayerByName(username);
            if (player != null) return player;

            // Attempt failed
            Client.LocalChat($"Player '{username}' not found.");
            return null;
        }
    }
}

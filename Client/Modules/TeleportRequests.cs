using System.Collections.Generic;
using System.Linq;
using SSMP.Api.Client;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Client.Modules
{
    internal static class TeleportRequests
    {
        internal class Request
        {
            static ushort requestId = 0;
            public ushort RequestId;
            public ushort PlayerId;
            public bool Responded = false;
            public Request(ushort id)
            {
                PlayerId = id;
                RequestId = requestId++;
            }
        }

        static readonly List<Request> requests = [];

        public static void AddRequest(IClientPlayer player)
        {
            // Remove any old requests from player
            requests.RemoveAll(r => r.PlayerId == player.Id);

            // Add a new request
            var request = new Request(player.Id);
            requests.Add(request);

            // Notify
            var requestStr = $"{Common.ColoredUsername(player)} has requested to teleport to you. " +
                "Use /tpaccept or /tpdeny within 30 seconds to respond.";

            Client.LocalChat(requestStr);
        }

        public static void RespondToRequest(bool accepted, string username = "")
        {
            Request? request = null;

            // Find request by username
            if (username != "")
            {
                var player = Client.GetPlayerByName(username);
                if (player == null)
                {
                    Client.LocalChat($"Player '{username}' not found.");
                    return;
                }

                request = requests.LastOrDefault(r => r.PlayerId == player.Id);
                if (request == null)
                {
                    Client.LocalChat($"{Common.ColoredUsername(player)} hasn't sent you a request.");
                    return;
                }
            }
            // Find last request, stack style
            else
            {
                if (requests.Count == 0)
                {
                    Client.LocalChat("You don't have any pending teleport requests.");
                    return;
                }

                request = requests.Last();
                requests.RemoveAt(requests.Count - 1);
            }

            request.Responded = true;

            // Send denied message
            if (!accepted)
            {
                PacketSender.SendMessage(request.PlayerId, Messages.TeleportDenied);
                return;
            }

            PacketSender.SendTeleportAccept(request);
        }
    }
}

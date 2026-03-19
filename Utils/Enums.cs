using System;
using System.Collections.Generic;
using System.Text;

namespace SSMPUtils.Utils
{
    internal enum PacketIDs
    {
        Huddle,
        TeleportRequest,
        TeleportAccept,
        Message,
        PlayerDeath
    }

    internal enum Messages
    {
        TeleportDenied,
    }
}

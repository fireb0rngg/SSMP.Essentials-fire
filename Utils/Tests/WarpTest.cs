using SSMPUtils.Client;
using SSMPUtils.Client.Packets;
using SSMPUtils.Server.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMPUtils.Utils.Tests
{
    internal class WarpTest : BaseTest
    {
        public override KeyCode KeyCode => KeyCode.Alpha1;

        public override void Execute()
        {
            var packet = new TeleportPacket
            {
                Scene = "Bonetown",
                Position = new SSMP.Math.Vector2(100, 15),
            };

            PacketReceiver.OnHuddle(packet);
        }
    }
}

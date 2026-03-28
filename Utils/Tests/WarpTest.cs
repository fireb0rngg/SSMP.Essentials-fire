using SSMPEssentials.Client;
using SSMPEssentials.Client.Packets;
using UnityEngine;

namespace SSMPEssentials.Utils.Tests
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

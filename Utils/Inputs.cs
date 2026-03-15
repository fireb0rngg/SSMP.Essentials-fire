using SSMP_Utils.Client.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMP_Utils.Utils
{
    internal static class Inputs
    {

        static void FreecamMovement()
        {
            var distance = 0.1f;
            var controller = InputHandler.Instance.inputActions;

            if (controller.Dash.IsPressed) distance *= 4;

            if (controller.Left.IsPressed) Spectate.FreecamMovementVector.x = 0 - distance;
            else if (controller.Right.IsPressed) Spectate.FreecamMovementVector.x = distance;
            else Spectate.FreecamMovementVector.x = 0;

            if (controller.Down.IsPressed) Spectate.FreecamMovementVector.y = 0 - distance;
            else if (controller.Up.IsPressed) Spectate.FreecamMovementVector.y = distance;
            else Spectate.FreecamMovementVector.y = 0;
        }
        public static void Update()
        {
            if (Input.GetKeyDown(Config.SpectateNext)) Spectate.FocusOnPlayer(Spectate.MoveDir.Next);
            else if (Input.GetKeyDown(Config.SpectatePrevious)) Spectate.FocusOnPlayer(Spectate.MoveDir.Prev);
            else if (Input.GetKeyDown(Config.ExitSpectate)) Spectate.ReturnToSelf();
            else if (Input.GetKeyDown(Config.FreecamToggle))
            {
                if (Spectate.freecam) Spectate.ReturnToSelf();
                else Spectate.Freecam();
            }

            if (Spectate.freecam)
            {
                FreecamMovement();
            }
        }
    }
}

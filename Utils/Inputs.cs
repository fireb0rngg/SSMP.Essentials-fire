using InControl;
using UnityEngine;
using SSMPEssentials.Client.Modules;

namespace SSMPEssentials.Utils
{
    internal static class Inputs
    {

        static float MovementAmount(PlayerAction action, PlayerAction inverseAction)
        {
            var deadZone = 0.1f;
            if (action.Value > deadZone)
            {
                return action.Value;
            }
            else if (inverseAction > deadZone)
            {
                return 0 - inverseAction.Value;
            }
            else
            {
                return 0;
            }
        }

        static void FreecamMovement()
        {
            var distance = 6f;
            var controller = InputHandler.Instance.inputActions;
            
            if (controller.Dash.IsPressed) distance = 2f;

            Spectate.FreecamMovementVector.x = MovementAmount(controller.Right, controller.Left) / distance;
            Spectate.FreecamMovementVector.y = MovementAmount(controller.Up, controller.Down) / distance;
        }
        public static void Update()
        {
            if (Client.Client.api?.UiManager.ChatBox.IsOpen ?? false) return;
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

using System.Collections;
using UnityEngine;
using SSMP.Game;
using SSMP.Api.Server;
using SSMP.Api.Client;

namespace SSMPUtils.Utils
{
    internal class Common
    {
        public static GameObject HornetObject => HeroController.SilentInstance?.gameObject;

        public static IEnumerator SetHornetPosition(Vector2 location)
        {
            var hero = HeroController.instance;
            var game = GameManager.instance;
            var hornet = HornetObject;

            hornet.transform.SetPosition2D(location);
            hero.RelinquishControl();

            game.FinishedEnteringScene();
            hero.SetState(GlobalEnums.ActorStates.no_input);
            hero.ResetLook();
            hero.rb2d.isKinematic = false;
            hero.rb2d.linearVelocity = Vector2.zero;
            hero.HazardRespawnReset();
            yield return null;

            hero.SendHeroInPosition(forceDirect: false);

            yield return new WaitForSeconds(0.3f);
            GCManager.Collect();
            yield return null;

            hero.proxyFSM.SendEvent("HeroCtrl-HazardRespawned");
            hero.rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;
            hero.FinishedEnteringScene(setHazardMarker: false);

            hero.RegainControl();
        }

        public static string TextColor(string text,  Colors color)
        {
            var colorStr = color switch
            {
                Colors.White => "white",
                Colors.Black => "black",
                Colors.Orange => "orange",
                Colors.Purple => "purple",
                Colors.Blue => "blue",
                Colors.Green => "green",
                Colors.Red => "red",
                Colors.Yellow => "yellow",
                _ => "white"
            };

            return $"<color=\"{colorStr}\">{text}</color>";
        }

        public static string ColoredUsername(IServerPlayer? player, Colors defaultColor = Colors.White)
        {
            if (player == null) return TextColor("Unknown Player", defaultColor);

            var username = player.Username;
            return ColoredUsername(username, player.Team, defaultColor);
        }

        public static string ColoredUsername(IClientPlayer? player, Colors defaultColor = Colors.White)
        {
            if (player == null) return TextColor("Unknown Player", defaultColor);

            var username = player.Username;
            return ColoredUsername(username, player.Team, defaultColor);
        }

        static string ColoredUsername(string username, Team team, Colors defaultColor)
        {
            return team switch
            {
                Team.Lifeblood => TextColor(username, Colors.Blue),
                Team.Moss => TextColor(username, Colors.Green),
                Team.Grimm => TextColor(username, Colors.Red),
                Team.Hive => TextColor(username, Colors.Orange),
                _ => TextColor(username, defaultColor),
            };
        }
    }
}

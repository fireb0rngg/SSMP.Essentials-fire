using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMP_Utils.Utils
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
    }
}

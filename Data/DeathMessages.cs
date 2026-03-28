using System.Collections.Generic;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Data
{
    internal static class DeathMessages
    {
        public static readonly Dictionary<CauseOfDeath, NoRepeat<string>> Messages = new()
        {
            { CauseOfDeath.Generic, new NoRepeat<string> {
                "died of unnatural causes",
                "unalived",
                "was killed",
                "decided to set up a Cocoon Skip",
                "took damage equal to or more than their current HP"
            } },
            { CauseOfDeath.Player, new NoRepeat<string> {
                "lost a duel to",
                "made masterful use of their vulnerability frames while fighting",
                "failed to negotiate with",
                "regrettably ended their encounter with",
                "fell to one of the classic blunders while fighting"
            } },
            { CauseOfDeath.Enemy, new NoRepeat<string> {
                "had an intimate encounter with an enemy",
                "made masterful use of their vulnerability frames",
                "didn't see the enemy's final form",
                "mistook an enemy hitbox for a wall",
                "was squashed by a common bug",
            } },
            { CauseOfDeath.Spikes, new NoRepeat<string> {
                "got impaled by spikes",
                "didn't see the sharp things",
                "fought the spikes and lost",
                "attempted rudimentary acupuncture",
                "was pricked to death"
            } },
            { CauseOfDeath.Acid, new NoRepeat<string> {
                "forgot to equip Isma's Tear before hopping in acid",
                "mistook sulfuric acid for water",
                "tried to swim in acid",
                "got into a fight with a pool of liquid"
            } },
            { CauseOfDeath.Lava, new NoRepeat<string> {
                "discovered what the sun feels like",
                "had a charring revelation",
                "tried to swim in lava",
                "went up in flames",
                "discovered the floor was lava"
            } },
            { CauseOfDeath.Pit, new NoRepeat<string> {
                "fell down a hole",
                "never hit the bottom",
                "saw a giant ravine and decided to jump",
                "hit the ground too hard",
                "was doomed to fall"
            } },
            { CauseOfDeath.Coal, new NoRepeat<string> {
                "got hit in the head by a lump of coal",
                "shouldn't have collected all those hot rocks",
                "was burned to a crisp by a chunk of rock",
                "discovered that coals are hot"
            } },
            { CauseOfDeath.Zap, new NoRepeat<string> {
                "was electrocuted to death",
                "was utterly shocked",
                "became one with the Voltvyrm"
            } },
            { CauseOfDeath.Explosion, new NoRepeat<string> {
                "went Last Judge mode and blew up",
                "was blown to fluff",
                "looked back at the explosion",
                "went out with a bang"
            } },
            { CauseOfDeath.Sink, new NoRepeat<string> {
                "sunk so low that they couldn't get out",
                "fell out of the world"
            } },
            { CauseOfDeath.Steam, new NoRepeat<string> {
                "took a ride in a tea kettle",
                "fell into a pressure cooker",
                "got a face full of hot air",
                "was caught venting",
                "was blown away by the amount of steam in the organ"
            } },
            { CauseOfDeath.Frost, new NoRepeat<string> {
                "chilled out a little too hard",
                "froze up",
                "froze to death",
                "forgot to wear a coat",
                "became an icicle"
            } }
        };
    }
}

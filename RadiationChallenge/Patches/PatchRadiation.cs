using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadiationChallenge.Patches
{
    public class PatchRadiation
    {
        public static float GetRadiativeDepth()
        {
            // A % of how strong the radiation is compared to max radiation
            float radiationStrengthPerc = LeakingRadiation.main.currentRadius / LeakingRadiation.main.kMaxRadius;

            // How deep the radiation can reach
            return (EntryPoint.config.radiativeDepth * 2) * radiationStrengthPerc;
        }

        /// <summary>
        /// Readjust the radiation values dependent on where the player is
        /// </summary>
        [HarmonyPostfix]
        public static void Update(PlayerDistanceTracker __instance)
        {
            // If the object is null, ship hasn't exploded yet or radius is too small
            if (LeakingRadiation.main == null || !CrashedShipExploder.main.IsExploded() || !GameModeUtils.HasRadiation())
            {
                return;
            }

            float radiationDepth = GetRadiativeDepth();

            if (radiationDepth < 1)
            {
                return;
            }

            // How deep the player is
            float playerDepth = Math.Max(0, -Player.main.transform.position.y);

            // If they are deeper than the radiation, return
            if (playerDepth > radiationDepth)
            {
                return;
            }

            // A % of how close they are to getting out of radiation
            float playerRadiationStrength = playerDepth / radiationDepth;

            var distance = __instance.GetType().GetField("_distanceToPlayer", System.Reflection.BindingFlags.NonPublic
        | System.Reflection.BindingFlags.Instance);
            var nearby = __instance.GetType().GetField("_playerNearby", System.Reflection.BindingFlags.NonPublic
        | System.Reflection.BindingFlags.Instance);

            // The new distance
            float playerDistance = __instance.maxDistance * playerRadiationStrength;
            // They're always nearby
            nearby.SetValue(__instance, true);
            // Get the current range
            float cDistance = (float)distance.GetValue(__instance);

            // Set the value that's the closest
            distance.SetValue(__instance, Math.Min(cDistance, playerDistance));
        }
    }
}
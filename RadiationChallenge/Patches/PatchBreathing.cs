using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RadiationChallenge.Patches
{
    public class PatchBreathing
    {
        /// <summary>
        /// If the ship has exploded, is leaking radiation, player is in the air, not inside a base,
        /// and not inside the sub
        /// </summary>
        private static bool IsRadiativeAir(Player player)
        {
            if (!IsCurrentlyRadiative())
            {
                return false;
            }

            // If the player is underwater, inside their base, inside a submersible or inside the
            // escape pod
            if (player.transform.position.y < -1 || player.IsInside())
            {
                return false;
            }

            return true;
        }

        private static bool IsCurrentlyRadiative()
        {
            // If ship hasn't exploded yet
            if (CrashedShipExploder.main == null || !CrashedShipExploder.main.IsExploded() || !GameModeUtils.HasRadiation())
            {
                return false;
            }

            // If the radiation is non-existant
            if (LeakingRadiation.main == null || PatchRadiation.GetRadiativeDepth() < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Prevent PipeSurfaceFloater from providing oxygen when in close range
        /// </summary>
        public static bool GetProvidesOxygen(PipeSurfaceFloater __instance, ref bool __result)
        {
            if (!IsCurrentlyRadiative())
            {
                // Not radiative, so don't handle
                return true;
            }

            if ((__instance.floater.transform.position - LeakingRadiation.main.transform.position).magnitude >= LeakingRadiation.main.currentRadius)
            {
                // Out of range from radiation
                return true;
            }

            // Don't let it provide oxygen
            __result = false;
            return false;
        }

        /// <summary>
        /// Called when it wants to check about adding air to the player, we cancel if the air is bad
        /// </summary>
        [HarmonyPrefix]
        public static bool AddOxygenAtSurface(OxygenManager __instance, ref float timeInterval)
        {
            Player player = Player.main;

            // If this is the wrong oxygen manager, or air isnt bad
            if (player.oxygenMgr != __instance || !IsRadiativeAir(player))
            {
                return true;
            }

            // Cancel the method as they cannot breathe
            return false;
        }

        /// <summary>
        /// Called when player surfaces, we override this because breathing hard on surface when you
        /// can't breathe is...
        /// </summary>
        [HarmonyPrefix]
        public static bool PlayReachSurfaceSound(WaterAmbience __instance)
        {
            if (!IsRadiativeAir(Player.main))
            {
                return true;
            }

            var time = __instance.GetType().GetField("timeReachSurfaceSoundPlayed", System.Reflection.BindingFlags.NonPublic
        | System.Reflection.BindingFlags.Instance);

            if (Time.time < (float)time.GetValue(__instance) + 1f)
            {
                return false;
            }

            // Skip other sounds as they are dependent on breathing

            time.SetValue(__instance, Time.time);
            __instance.reachSurfaceWithTank.Play(); // Different sound so no splash

            return false;
        }

        /// <summary>
        /// Overrides a check in Player, this controls if the player can breathe fresh air where they
        /// are currently
        /// </summary>
        [HarmonyPrefix]
        public static bool CanBreathe(Player __instance, ref bool __result)
        {
            if (IsRadiativeAir(__instance))
            {
                __result = false;
                return false;
            }

            // Don't cancel
            return true;
        }

        /// <summary>
        /// This is overriden as the breath period on surface would be a very large value otherwise
        /// </summary>
        [HarmonyPostfix]
        public static void GetBreathPeriod(ref float __result)
        {
            if (!IsRadiativeAir(Player.main))
            {
                return;
            }

            // Set their breath to the lowest value, 3 seconds is better than 9999 seconds
            __result = Math.Min(3f, __result);
        }
    }
}
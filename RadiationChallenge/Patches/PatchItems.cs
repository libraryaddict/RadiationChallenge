using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RadiationChallenge.Patches
{
    public class PatchItems
    {
        /// <summary>
        /// If the player is in the outside air and there is radiation active
        /// </summary>
        private static bool IsRadiative()
        {
            // If player can't be on island, an exosuit might bypass this but.
            if (Player.main.IsInside() || Player.main.transform.position.y < -1)
            {
                return false;
            }

            // If radiation is at a small level
            if (PatchRadiation.GetRadiativeDepth() < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called when player knifes an object, cancel if it's a food plant
        /// </summary>
        [HarmonyPrefix]
        public static bool GiveResourceOnDamage(ref GameObject target)
        {
            if (!IsRadiative()) // If island isn't radiative, return without cancelling
            {
                return true;
            }

            TechType techType = CraftData.GetTechType(target);

            switch (techType)
            {
                case TechType.PurpleVegetablePlant:
                case TechType.MelonPlant:
                case TechType.BulboTree:
                    return false; // Don't let them harvest the plants
                default:
                    return true;
            }
        }

        /// <summary>
        /// Disable pickups on the island while radiation is active
        /// </summary>
        [HarmonyPrefix]
        public static bool HandleItemPickup(ref PickPrefab __instance)
        {
            if (!IsRadiative()) // If not radiative
            {
                return true;
            }

            switch (__instance.pickTech)
            {
                // Don't let the player pick up edible plants
                case TechType.Melon:
                case TechType.SmallMelon:
                case TechType.HangingFruit:
                case TechType.PurpleVegetable:
                    return false;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Controls the fabricator inside the escape pod, disables food crafting once ship has exploded
        /// </summary>
        [HarmonyPrefix]
        public static bool IsCraftRecipeFulfilled(ref TechType techType, ref bool __result)
        {
            // If they are not in the escape pod, return early without taking over method and If the
            // ship hasn't exploded, don't do anything
            if (!Player.main.escapePod.value || !CrashedShipExploder.main.IsExploded())
            {
                return true;
            }

            // Switch between the tech types, we could use a name check but that's always prone to
            // random edge cases. Better we just stick with the list of known foods
            switch (techType)
            {
                case TechType.Bleach:
                case TechType.FilteredWater:
                case TechType.DisinfectedWater:
                case TechType.CookedPeeper:
                case TechType.CookedHoleFish:
                case TechType.CookedGarryFish:
                case TechType.CookedReginald:
                case TechType.CookedBladderfish:
                case TechType.CookedHoverfish:
                case TechType.CookedSpadefish:
                case TechType.CookedBoomerang:
                case TechType.CookedEyeye:
                case TechType.CookedOculus:
                case TechType.CookedHoopfish:
                case TechType.CookedSpinefish:
                case TechType.CookedLavaEyeye:
                case TechType.CookedLavaBoomerang:
                case TechType.CuredPeeper:
                case TechType.CuredHoleFish:
                case TechType.CuredGarryFish:
                case TechType.CuredReginald:
                case TechType.CuredBladderfish:
                case TechType.CuredHoverfish:
                case TechType.CuredSpadefish:
                case TechType.CuredBoomerang:
                case TechType.CuredEyeye:
                case TechType.CuredOculus:
                case TechType.CuredHoopfish:
                case TechType.CuredSpinefish:
                case TechType.CuredLavaEyeye:
                case TechType.CuredLavaBoomerang:
                    __result = false; // They may not craft this
                    return false; // Cancel method
                default:
                    return true; // Let method continue
            }
        }
    }
}
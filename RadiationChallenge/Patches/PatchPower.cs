using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadiationChallenge.Patches
{
    public class PatchPower
    {
        private static UnityEngine.Transform GetTransform(IPowerInterface powerInterface)
        {
            if (powerInterface is BatterySource)
            {
                return ((BatterySource)powerInterface).transform;
            }
            else if (powerInterface is PowerSource)
            {
                return ((PowerSource)powerInterface).transform;
            }
            else
               if (powerInterface is PowerRelay)
            {
                return ((PowerRelay)powerInterface).transform;
            }

            return null;
        }

        private static void AddEnergy(UnityEngine.Transform transform, ref float amount)
        {
            if (!RadiationUtils.GetInAnyRadiation(transform))
            {
                return;
            }

            amount *= EntryPoint.config.radiativePowerAddMultiplier;
        }

        private static void ConsumeEnergy(UnityEngine.Transform transform, ref float amount)
        {
            if (!RadiationUtils.GetInAnyRadiation(transform))
            {
                return;
            }

            amount *= EntryPoint.config.radiativePowerConsumptionMultiplier;
        }

        [HarmonyPrefix]
        public static void ConsumeEnergyBase(ref IPowerInterface powerInterface, ref float amount)
        {
            ConsumeEnergy(GetTransform(powerInterface), ref amount);
        }

        [HarmonyPrefix]
        public static void AddEnergyBase(ref IPowerInterface powerInterface, ref float amount)
        {
            AddEnergy(GetTransform(powerInterface), ref amount);
        }

        [HarmonyPrefix]
        public static void ConsumeEnergyTool(ref float amount)
        {
            ConsumeEnergy(Player.main.transform, ref amount);
        }

        [HarmonyPrefix]
        public static void AddEnergyTool(ref float amount)
        {
            AddEnergy(Player.main.transform, ref amount);
        }

        [HarmonyPrefix]
        public static void ConsumeEnergyVehicle(Vehicle __instance, ref float amount)
        {
            ConsumeEnergy(__instance.transform, ref amount);
        }

        [HarmonyPrefix]
        public static void AddEnergyVehicle(Vehicle __instance, ref float amount)
        {
            AddEnergy(__instance.transform, ref amount);
        }
    }
}
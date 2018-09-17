using Harmony;
using RadiationChallenge.Patches;
using System;
using System.IO;
using System.Reflection;
using Oculus.Newtonsoft.Json;

namespace RadiationChallenge
{
    public class EntryPoint
    {
        public static RadiationConfig config;
        private static readonly string configPath = @"./QMods/RadiationChallenge/config.json";

        private static void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                config = new RadiationConfig();
                return;
            }

            var json = File.ReadAllText(configPath);
            config = JsonConvert.DeserializeObject<RadiationConfig>(json);
        }

        public static void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, json);
        }

        public static void Patch()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("com.libraryaddict.challenge.radiation");

            if (harmony == null)
            {
                return;
            }

            LoadConfig();
            SaveConfig();

            if (config.explosionDepth > 0)
            {
                harmony.Patch(typeof(CrashedShipExploder).GetMethod("CreateExplosiveForce", BindingFlags.NonPublic | BindingFlags.Instance),
                     null, new HarmonyMethod(typeof(PatchExplosion).GetMethod("CreateExplosiveForce")));
            }

            if (config.poisonedAir)
            {
                harmony.Patch(AccessTools.Method(typeof(Player), "CanBreathe"),
                    new HarmonyMethod(typeof(PatchBreathing).GetMethod("CanBreathe")), null);

                harmony.Patch(AccessTools.Method(typeof(Player), "GetBreathPeriod"), null,
                    new HarmonyMethod(typeof(PatchBreathing).GetMethod("GetBreathPeriod")));

                harmony.Patch(AccessTools.Method(typeof(OxygenManager), "AddOxygenAtSurface"),
                    new HarmonyMethod(typeof(PatchBreathing).GetMethod("AddOxygenAtSurface")), null);

                harmony.Patch(AccessTools.Method(typeof(WaterAmbience), "PlayReachSurfaceSound"),
                    new HarmonyMethod(typeof(PatchBreathing).GetMethod("PlayReachSurfaceSound")), null);

                harmony.Patch(AccessTools.Method(typeof(PipeSurfaceFloater), "GetProvidesOxygen"),
                    new HarmonyMethod(typeof(PatchBreathing).GetMethod("GetProvidesOxygen")), null);
            }

            if (config.radiationWarning)
            {
                harmony.Patch(AccessTools.Method(typeof(uGUI_RadiationWarning), "IsRadiated"),
                    new HarmonyMethod(typeof(PatchRadiation).GetMethod("IsRadiated")), null);
            }

            if (config.radiativeDepth > 0)
            {
                harmony.Patch(AccessTools.Method(typeof(RadiatePlayerInRange), "Radiate"),
                    new HarmonyMethod(typeof(PatchRadiation).GetMethod("Radiate")), null);

                harmony.Patch(AccessTools.Method(typeof(DamagePlayerInRadius), "DoDamage"),
                    new HarmonyMethod(typeof(PatchRadiation).GetMethod("DoDamage")), null);

                if (config.radiativePowerAddMultiplier > 0)
                {
                    harmony.Patch(AccessTools.Method(typeof(PowerSystem), "AddEnergy"),
                        new HarmonyMethod(typeof(PatchPower).GetMethod("AddEnergyBase")), null);

                    harmony.Patch(AccessTools.Method(typeof(EnergyMixin), "AddEnergy"),
                        new HarmonyMethod(typeof(PatchPower).GetMethod("AddEnergyTool")), null);

                    harmony.Patch(AccessTools.Method(typeof(Vehicle), "AddEnergy", new Type[] { typeof(float) }),
                        new HarmonyMethod(typeof(PatchPower).GetMethod("AddEnergyVehicle")), null);
                }

                if (config.radiativePowerConsumptionMultiplier > 0)
                {
                    harmony.Patch(AccessTools.Method(typeof(PowerSystem), "ConsumeEnergy"),
                        new HarmonyMethod(typeof(PatchPower).GetMethod("ConsumeEnergyBase")), null);

                    harmony.Patch(AccessTools.Method(typeof(EnergyMixin), "ConsumeEnergy"),
                        new HarmonyMethod(typeof(PatchPower).GetMethod("ConsumeEnergyTool")), null);

                    harmony.Patch(AccessTools.Method(typeof(Vehicle), "ConsumeEnergy", new Type[] { typeof(float) }),
                        new HarmonyMethod(typeof(PatchPower).GetMethod("ConsumeEnergyVehicle")), null);
                }
            }

            if (config.disableFabricatorFood)
            {
                harmony.Patch(AccessTools.Method(typeof(CrafterLogic), "IsCraftRecipeFulfilled"),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("IsCraftRecipeFulfilled")), null);
            }

            if (config.preventPreRadiativeFoodPickup || config.preventRadiativeFoodPickup)
            {
                // Disable the hover hand, and disable ability to click
                harmony.Patch(AccessTools.Method(typeof(PickPrefab), "OnHandHover"),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("HandleItemPickup")), null);
                harmony.Patch(AccessTools.Method(typeof(PickPrefab), "OnHandClick"),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("HandleItemPickup")), null);

                harmony.Patch(AccessTools.Method(typeof(RepulsionCannon), "ShootObject"),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("ShootObject")), null);

                harmony.Patch(AccessTools.Method(typeof(PropulsionCannon), "ValidateObject"),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("ValidateObject")), null);

                // Don't let player smash the resources for seeds
                harmony.Patch(typeof(Knife).GetMethod("GiveResourceOnDamage", BindingFlags.NonPublic | BindingFlags.Instance),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("GiveResourceOnDamage")), null);
            }

            Console.WriteLine("[RadiationChallenge] Patched");
        }
    }
}
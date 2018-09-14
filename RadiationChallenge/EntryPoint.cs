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
        private static readonly String configPath = @"./QMods/RadiationChallenge/config.json";

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
            var json = JsonConvert.SerializeObject(config);
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
                harmony.Patch(typeof(CrashedShipExploder).GetMethod("ShakePlayerCamera", BindingFlags.NonPublic | BindingFlags.Instance),
                     null, new HarmonyMethod(typeof(PatchExplosion).GetMethod("ShakePlayerCamera")));
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

            if (config.radiativeDepth > 0)
            {
                harmony.Patch(AccessTools.Method(typeof(PlayerDistanceTracker), "Update"), null,
                    new HarmonyMethod(typeof(PatchRadiation).GetMethod("Update")));
            }

            if (config.disableFabricatorFood)
            {
                harmony.Patch(AccessTools.Method(typeof(CrafterLogic), "IsCraftRecipeFulfilled"),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("IsCraftRecipeFulfilled")), null);
            }

            if (config.preventRadiativeFoodPickup)
            {
                harmony.Patch(AccessTools.Method(typeof(PickPrefab), "OnHandHover"),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("HandleItemPickup")), null);
                harmony.Patch(AccessTools.Method(typeof(PickPrefab), "OnHandClick"),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("HandleItemPickup")), null);

                harmony.Patch(typeof(Knife).GetMethod("GiveResourceOnDamage", BindingFlags.NonPublic | BindingFlags.Instance),
                    new HarmonyMethod(typeof(PatchItems).GetMethod("GiveResourceOnDamage")), null);
            }

            Console.WriteLine("[RadiationChallenge] Patched");
        }
    }
}
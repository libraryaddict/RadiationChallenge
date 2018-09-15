using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadiationChallenge.Patches
{
    public class PatchExplosion
    {
        /// <summary>
        /// Patch explosion to kill below a certain depth
        /// </summary>
        [HarmonyPostfix]
        public static void CreateExplosiveForce() // Should only be called when aurora explodes
        {
            if (Player.main.transform.position.y <= -EntryPoint.config.explosionDepth) // If they are below a depth of X
            {
                return;
            }

            Player.main.liveMixin.Kill(DamageType.Explosive); // They died from explosion
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadiationChallenge
{
    public class RadiationUtils
    {
        /// <summary>
        /// How deep into the ocean the radiation can penetrate.
        ///
        /// From 1 to X
        /// </summary>
        public static float GetRadiationDepth()
        {
            // A % of how strong the radiation is compared to max radiation
            float radiationStrengthPerc = LeakingRadiation.main.currentRadius / LeakingRadiation.main.kMaxRadius;

            // How deep the radiation can reach
            return EntryPoint.config.radiativeDepth * radiationStrengthPerc;
        }

        public static bool GetRadiationActive()
        {
            // If LeakingRadiation isn't null, ship has exploded and radiation is enabled
            return LeakingRadiation.main != null && CrashedShipExploder.main.IsExploded() && GameModeUtils.HasRadiation() && LeakingRadiation.main.currentRadius > 1;
        }

        public static bool GetSurfaceRadiationActive()
        {
            return GetRadiationActive() && GetRadiationDepth() > 1;
        }

        public static bool GetInShipsRadiation(UnityEngine.Transform transform)
        {
            return GetRadiationActive() && (transform.position - LeakingRadiation.main.transform.position).magnitude <= LeakingRadiation.main.currentRadius;
        }

        public static bool GetInAnyRadiation(UnityEngine.Transform transform)
        {
            if (!GetRadiationActive())
            {
                return false;
            }

            float depth = GetRadiationDepth();

            if (depth > 1 && transform.position.y >= -depth)
            {
                return true;
            }

            return (transform.position - LeakingRadiation.main.transform.position).magnitude <= LeakingRadiation.main.currentRadius;
        }

        public static bool GetInSurfaceRadiation(UnityEngine.Transform transform)
        {
            if (!GetRadiationActive())
            {
                return false;
            }

            float depth = GetRadiationDepth();

            return depth > 1 && transform.position.y >= -depth;
        }
    }
}
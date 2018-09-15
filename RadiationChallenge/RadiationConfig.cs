using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadiationChallenge
{
    public class RadiationConfig
    {
        public float radiativeDepth = 30;
        public float explosionDepth = 50;
        public float radiativePowerConsumptionMultiplier = 5;
        public float radiativePowerAddMultiplier = 0.2f;

        public bool disableFabricatorFood = true;
        public bool preventPreRadiativeFoodPickup = true;
        public bool preventRadiativeFoodPickup = true;
        public bool poisonedAir = true;
    }
}
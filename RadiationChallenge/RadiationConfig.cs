using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadiationChallenge
{
    public class RadiationConfig
    {
        public int radiativeDepth = 30;
        public int explosionDepth = 50;

        public bool disableFabricatorFood = true;
        public bool preventRadiativeFoodPickup = true;
        public bool poisonedAir = true;
    }
}
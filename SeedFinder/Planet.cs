using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeedFinder
{
    public class Planet
    {
        public int id;
        public string name;
        public string type;
        public float distanceToSun;
        public float landPercent;
        public float windStrength;
        public float solarStrength;
        public string singularity;
        public string orbitalInclination;
        public string axialInclination;
        public Planet(PlanetData planet)
        {
            this.id = planet.id;
            this.name = planet.name;
            this.type = planet.typeString;
            this.distanceToSun = planet.sunDistance;
            this.landPercent = planet.landPercent;
            this.singularity = planet.singularity.ToString();
            this.windStrength = planet.windStrength;
            this.solarStrength = planet.luminosity;
            this.orbitalInclination = this.GetInclination(planet.orbitInclination);
            this.axialInclination = this.GetInclination(planet.obliquity);
        }

        protected string GetInclination(float input)
        {
            float absolute = Mathf.Abs(input);
            int inclinationMinutes = (int)absolute;
            int inclinationSeconds = (int)((absolute - (float)inclinationMinutes) * 60f);
            if (input < 0f)
            {
                inclinationMinutes = -inclinationMinutes;
            }
            return string.Format("{0}° {1}′", inclinationMinutes, inclinationSeconds);
        }
    }
}

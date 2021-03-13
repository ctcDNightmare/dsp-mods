using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinder
{
    public class Star
    {
        public int id;
        public string name;
        public bool isHome = false;
        public float luminosity;
        public string type;
        public float distance;
        public List<Planet> planets;

        public Star(StarData star)
        {
            this.id = star.id;
            this.name = star.name;
            this.isHome = (star.id == GameMain.galaxy.birthStarId);
            this.luminosity = star.luminosity;
            this.type = star.typeString;
            this.planets = new List<Planet>();
            this.distance = GetDistance(star.uPosition);
            foreach (PlanetData planet in star.planets)
            {
                this.planets.Add(new Planet(planet));
            }

        }

        protected float GetDistance(VectorLF3 position)
        {
            return (float)(position - new VectorLF3(0f, 0f, 0f)).magnitude / 2400000.0f;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;


namespace SeedFinder
{
    public class DSPMap: MonoBehaviour
    {
        public List<Star> stars;
        public string title;
        public string author;
        public string note;
        public string version;
        public string seed;
        public float resourceMultiplier;
        public int starcount;
        public List<string> tags;
        public List<string> mods;

        public DSPMap () {
           
        }

        public StarData[] StarsRaw
        {
            set
            {
                foreach (StarData star in value)
                {
                    stars.Add(new Star(star));
                }
            }
        }
    }

    public class Star
    {
        public int id;
        public string name;
        public bool isHome = false;
        public float luminosity;
        public string type;
        public double distance;

        public Star(StarData star)
        {
            this.id = star.id;
            this.name = star.name;
            this.isHome = (star.id == GameMain.galaxy.birthStarId);
            this.luminosity = star.luminosity;
            this.type = star.typeString;
        }


    }
}
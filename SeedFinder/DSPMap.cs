using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace SeedFinder
{
    public class DSPMap: MonoBehaviour
    {
        protected StringBuilder sb;

        public string title;
        public string author;
        public string note;
        public string version;
        public string seed;
        public float resourceMultiplier;
        public int starcount;
        public List<string> tags;
        public List<string> mods;
        public List<Star> stars;

        public DSPMap () {
            tags = new List<string>();
            mods = new List<string>();
            stars = new List<Star>();
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

        public string ToJson()
        {
            sb = new StringBuilder();
            sb.Append("{");
            sb.Append(this.ToJson(this.title, "title"));
            sb.Append(this.ToJson(this.author, "author"));
            sb.Append(this.ToJson(this.note, "note"));
            sb.Append(this.ToJson(this.version, "version"));
            sb.Append(this.ToJson(this.seed, "seed"));
            sb.Append(this.ToJson(this.resourceMultiplier, "resourceMultiplier"));
            sb.Append(this.ToJson(this.starcount, "starcount"));
            sb.Append(this.ToJson(this.tags, "tags"));
            sb.Append(this.ToJson(this.mods, "mods"));
            sb.Append(this.ToJson(this.stars, "stars", true));
            sb.Append("}");
            return sb.ToString();
        }

        protected string ToJson(string value, string key = null, bool last = false)
        {
            StringBuilder sb = new StringBuilder();
            if (key != null)
            {
                sb.Append("\"" + key + "\":");
            }
            sb.Append("\"" + value + "\"");
            if (!last)
            {
                sb.Append(",");
            }
            return sb.ToString();
        }

        protected string ToJson(float value, string key = null, bool last = false)
        {
            StringBuilder sb = new StringBuilder();
            if (key != null)
            {
                sb.Append("\"" + key + "\":");
            }
            sb.Append(value.ToString("F"));
            if (!last)
            {
                sb.Append(",");
            }
            return sb.ToString();
        }

        protected string ToJson(int value, string key = null, bool last = false)
        {
            StringBuilder sb = new StringBuilder();
            if (key != null)
            {
                sb.Append("\"" + key + "\":");
            }
            sb.Append(value.ToString());
            if (!last)
            {
                sb.Append(",");
            }
            return sb.ToString();
        }

        protected string ToJson(bool value, string key = null, bool last = false)
        {
            StringBuilder sb = new StringBuilder();
            if (key != null)
            {
                sb.Append("\"" + key + "\":");
            }
            sb.Append((value == true ? "true" : "false" ));
            if (!last)
            {
                sb.Append(",");
            }
            return sb.ToString();
        }

        protected string ToJson(List<string> values, string key = null, bool last = false)
        {
            StringBuilder sb = new StringBuilder();
            if (key != null)
            {
                sb.Append("\"" + key + "\":");
            }
            sb.Append("[");
            for (var i = 0; i < values.Count; i++)
            {
                sb.Append(this.ToJson(values[i], null, (i == values.Count - 1)));
            }
            sb.Append("]");
            
            if (!last)
            {
                sb.Append(",");
            }
            return sb.ToString();
        }

        protected string ToJson(List<Star> values, string key = null, bool last = false)
        {
            StringBuilder sb = new StringBuilder();
            if (key != null)
            {
                sb.Append("\"" + key + "\":");
            }
            sb.Append("[");
            for (var i = 0; i < values.Count; i++)
            {
                bool lastStar = (i == values.Count - 1);
                sb.Append("{");
                sb.Append(this.ToJson(values[i].id, "id"));
                sb.Append(this.ToJson(values[i].name, "name"));
                sb.Append(this.ToJson(values[i].isHome, "isHome"));
                sb.Append(this.ToJson(values[i].luminosity, "luminosity"));
                sb.Append(this.ToJson(values[i].type, "type"));
                sb.Append(this.ToJson(values[i].distance, "distance"));
                sb.Append(this.ToJson(values[i].planets, "planets", true));
                sb.Append("}");
                if (!lastStar)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");

            if (!last)
            {
                sb.Append(",");
            }
            return sb.ToString();
        }

        protected string ToJson(List<Planet> values, string key = null, bool last = false)
        {
            StringBuilder sb = new StringBuilder();
            if (key != null)
            {
                sb.Append("\"" + key + "\":");
            }
            sb.Append("[");
            for (var i = 0; i < values.Count; i++)
            {
                bool lastPlanet = i == (values.Count - 1);
                sb.Append("{");
                sb.Append(this.ToJson(values[i].id, "id"));
                sb.Append(this.ToJson(values[i].name, "name"));
                sb.Append(this.ToJson(values[i].type, "type"));
                sb.Append(this.ToJson(values[i].distanceToSun, "distanceToSun"));
                sb.Append(this.ToJson(values[i].landPercent, "landPercent"));
                sb.Append(this.ToJson(values[i].singularity, "singularity"));
                sb.Append(this.ToJson(values[i].windStrength, "windStrength"));
                sb.Append(this.ToJson(values[i].solarStrength, "solarStrength"));
                sb.Append(this.ToJson(values[i].orbitalInclination, "orbitalInclination"));
                sb.Append(this.ToJson(values[i].axialInclination, "axialInclination", true));
                sb.Append("}");
                if (!lastPlanet)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");

            if (!last)
            {
                sb.Append(",");
            }
            return sb.ToString();
        }
    }
}
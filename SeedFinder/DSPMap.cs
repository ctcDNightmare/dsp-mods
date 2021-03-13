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
            sb = new StringBuilder();
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
            sb.Append("{");
            sb.Append("\"title\":\"" + this.title + "\",");
            sb.Append("\"author\":\"" + this.author + "\",");
            sb.Append("\"note\":\"" + this.note + "\",");
            sb.Append("\"version\":\"" + this.version + "\",");
            sb.Append("\"seed\":\"" + this.seed + "\",");
            sb.Append("\"resourceMultiplier\":" + this.resourceMultiplier.ToString("F") + ",");
            sb.Append("\"starcount\":" + this.starcount.ToString() + ",");
            sb.Append("\"tags\":[");
            if (this.tags.Count > 0)
            {
                for (var i = 0; i < this.tags.Count; i++)
                {
                    sb.Append("\"" + this.tags[i] + "\"");
                    if (i < this.tags.Count - 1)
                    {
                        sb.Append(",");
                    }
                }   
            }
            sb.Append("],");
            sb.Append("\"mods\":[");
            if (this.mods.Count > 0)
            {
                for (var i = 0; i < this.mods.Count; i++)
                {
                    sb.Append("\"" + this.mods[i] + "\"");
                    if (i < this.mods.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
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
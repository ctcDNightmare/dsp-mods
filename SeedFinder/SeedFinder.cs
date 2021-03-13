using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using FullSerializer;
using System.Security.Policy;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SeedFinder
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class SeedFinder : BaseUnityPlugin
    {
        // info
        protected const string pluginGuid = "dsp.dnightmare.seedfinder";
        protected const string pluginName = "SeedFinder";
        protected const string pluginVersion = "0.1.0";

        // config
        protected static bool cfgEnabled = true;
        protected static Boolean cfgKeepSearching = false;
        protected static bool cfgUnipolarEnabled = true;
        protected static int cfgUnipolarMinDistance = 30;
        protected static int cfgUnipolarMinAmount = 1;
        protected static bool cfgOTypeEnabled = true;
        protected static int cfgOTypeMinDistance = 30;
        protected static float cfgOTypeMinLuminosity = 2.3f;

        // ui
        protected static UIGalaxySelect uiGalaxySelect = null;
        protected static RectTransform triggerButton;

        // game
        protected static GalaxyData galaxy = null;
        protected static StarData startPlanet = null;
        protected static List<StarData> unipolarSources = new List<StarData>();
        protected static List<StarData> oTypeStars = new List<StarData>();

        // misc.
        protected static Boolean searchInProgress = false;
        protected static DSPMap map = null;
        protected static List<string> modList = new List<string>();
        
        new internal static BepInEx.Logging.ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(pluginName);

        void Awake()
        {
            // general
            cfgEnabled = Config.Bind<bool>("General", "enablePlugin", cfgEnabled, "Main toggle to enable/disable this plugin").Value;
            cfgKeepSearching = Config.Bind<bool>("General", "keepSearching", cfgKeepSearching, "After finding a good seed, export result and continue?").Value;

            // neutron star / block holes
            cfgUnipolarEnabled = Config.Bind<bool>("Unipolar", "enableSearch", cfgUnipolarEnabled, "Search for specific black holes / neutron stars").Value;
            cfgUnipolarMinDistance = Config.Bind<int>("Unipolar", "minDistance", cfgUnipolarMinDistance, "Minimum distance from startplanet.").Value;
            cfgUnipolarMinAmount = Config.Bind<int>("Unipolar", "amountValidDistance", cfgUnipolarMinAmount, "Minimum amount of black holes / neutron stars that must pass distance check.").Value;
            
            // o-type
            cfgOTypeEnabled = Config.Bind<bool>("OType", "enableSearch", cfgOTypeEnabled, "Search for specific O-type stars").Value;
            cfgOTypeMinDistance = Config.Bind<int>("OType", "minDistance", cfgOTypeMinDistance, "Minimum distance from startplanet").Value;
            cfgOTypeMinLuminosity = Config.Bind<float>("OType", "minLuminosity", cfgOTypeMinLuminosity, "Minimum luminosity").Value;

            if (cfgEnabled == true)
            {
                var harmony = new Harmony(pluginGuid);
                harmony.PatchAll(typeof(SeedFinder));
                Logger.LogInfo("Unipolar Search: " + (cfgUnipolarEnabled ? "enabled" : "disabled"));
                Logger.LogInfo("O-Type Search: " + (cfgOTypeEnabled ? "enabled" : "disabled"));
            }
        }

        void Update()
        {
            if (
                uiGalaxySelect == null || 
                searchInProgress == false || 
                galaxy == uiGalaxySelect.starmap.galaxyData
                )
            {
                return;
            }
            if (modList.Count == 0)
            {
                foreach (KeyValuePair<string, PluginInfo> plugin in BepInEx.Bootstrap.Chainloader.PluginInfos)
                {
                    string mod = plugin.Key + "-" + plugin.Value.Metadata.Version;
                    modList.Add(mod);
                }
            }
            galaxy = uiGalaxySelect.starmap.galaxyData;
            startPlanet = galaxy.StarById(galaxy.birthStarId);
            Boolean isGoodSeed = CheckSeed();
            if (isGoodSeed)
            {
                if (!cfgKeepSearching)
                {
                    StopSearch();
                }
                Logger.LogInfo(Environment.NewLine + "--- Found something ---" + Environment.NewLine);
                map = new DSPMap();
                if (GameMain.data != null && !string.IsNullOrEmpty(GameMain.data.account.detail.userName))
                {
                    map.author = GameMain.data.account.detail.userName;
                }
                GameDesc newGameDesc = (GameDesc)typeof(UIGalaxySelect).GetField("gameDesc", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(uiGalaxySelect);
                map.title = newGameDesc.clusterString;
                map.seed = newGameDesc.galaxySeed.ToString("00000000");
                map.resourceMultiplier = newGameDesc.resourceMultiplier;
                map.version = GameConfig.gameVersion.ToFullString();
                map.starcount = galaxy.starCount;
                map.mods = modList;

                //Logger.LogInfo(JsonSerializer.Serialize(typeof(DSPMap), map));
                Logger.LogInfo(map.ToJson());
                
                /**Logger.LogInfo("Cluster " + map.title);
                foreach (StarData star in unipolarSources)
                {
                    Logger.LogInfo(star.name + " (" + star.typeString + ") @ " + DistanceFromStart(star).ToString("F3") + "AU");
                }
                foreach (StarData star in oTypeStars)
                {
                    Logger.LogInfo(star.name + " (" + star.dysonLumino + ") @ " + DistanceFromStart(star).ToString("F3") + "AU" + Environment.NewLine);
                }**/
                return;

            }
            uiGalaxySelect.Rerand();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "Rerand")]
        protected static void RerandPostfix(GameDesc ___gameDesc)
        {
            //Logger.LogInfo(___gameDesc.clusterString);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        protected static void OnOpenPostfix(ref UIGalaxySelect __instance)
        {
            Logger.LogDebug("We need a new button");
            CreateFindButton(__instance);
        }

        protected static Boolean CheckSeed()
        {
            return CheckUnipolarSources() && CheckOTypes();
        }

        protected static Boolean CheckUnipolarSources()
        {
            if (!cfgUnipolarEnabled)
            {
                return true;
            }
            unipolarSources = new List<StarData>();
            List<EStarType> validSources = new List<EStarType>(){
                EStarType.BlackHole,
                EStarType.NeutronStar
            };
            foreach (StarData star in galaxy.stars)
            {
                if (validSources.Contains(star.type) && DistanceFromStart(star) >= cfgUnipolarMinDistance)
                {
                    unipolarSources.Add(star);
                }
            }
            return unipolarSources.Count > cfgUnipolarMinAmount;
        }

        protected static bool CheckOTypes()
        {
            if (cfgOTypeEnabled == false)
            {
                return true;
            }
            oTypeStars = new List<StarData>();
            List<String> validTypes = new List<String>(){
                "O type star"
            };
            foreach (StarData star in galaxy.stars)
            {
                if (validTypes.Contains(star.typeString))
                {
                    if (DistanceFromStart(star) >= cfgOTypeMinDistance && star.dysonLumino >= cfgOTypeMinLuminosity)
                    {
                        oTypeStars.Add(star);
                    }
                }
            }
            return (oTypeStars.Count > 0);
        }

        protected static double DistanceFromStart(StarData target)
        {
            return (target.uPosition - startPlanet.uPosition).magnitude / 2400000.0f;
        }

        protected static void CreateFindButton(UIGalaxySelect __instance)
        {
            if (uiGalaxySelect == null)
            {
                uiGalaxySelect = __instance;
            }
            if (triggerButton == null)
            {
                RectTransform prefab = GameObject.Find("Galaxy Select/random-button").GetComponent<RectTransform>();

                triggerButton = GameObject.Instantiate<RectTransform>(prefab);
                triggerButton.gameObject.name = "dnightmare-export-button";
                triggerButton.transform.SetParent(uiGalaxySelect.transform);
                Vector3 newPosition = prefab.transform.position;
                newPosition.x -= 1f;
                Vector3 newScale = prefab.transform.localScale;

                triggerButton.transform.position = newPosition;
                triggerButton.transform.localScale = newScale;
                triggerButton.GetComponentInChildren<Text>().text = "Find";
                triggerButton.GetComponentInChildren<Text>().color = new Color(0.1f, 0.1f, 0.75f);
                triggerButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                triggerButton.GetComponent<Button>().onClick.AddListener(delegate
                {
                    ToggleSearch();
                });
            }

        }

        protected static void ToggleSearch()
        {
            if (searchInProgress)
            {
                StopSearch();
            } else
            {
                StartSearch();
            }
        }

        protected static void StartSearch()
        {
            searchInProgress = true;
            triggerButton.GetComponentInChildren<Text>().text = "Stop";
            uiGalaxySelect.Rerand();
        }

        protected static void StopSearch()
        {
            searchInProgress = false;
            triggerButton.GetComponentInChildren<Text>().text = "Find";
        }

        protected static void OutputSeedInfo(GalaxyData galaxy)
        {
            Logger.LogInfo(galaxy.starCount.ToString());
            foreach(StarData star in galaxy.stars)
            {
                Logger.LogInfo(star.name);
                foreach(PlanetData planet in star.planets)
                {
                    if (planet.singularity != EPlanetSingularity.None)
                    {
                        Logger.LogInfo(planet.name + ": " + planet.singularity.ToString());
                        if (planet.type != EPlanetType.Gas)
                        {
                            PlanetAlgorithm planetAlgorithm = PlanetModelingManager.Algorithm(planet);
                            planet.data = new PlanetRawData(planet.precision);
                            planet.modData = planet.data.InitModData(planet.modData);
                            planet.data.CalcVerts();
                            planet.aux = new PlanetAuxData(planet);
                            planetAlgorithm.GenerateTerrain(planet.mod_x, planet.mod_y);
                            planetAlgorithm.CalcWaterPercent();
                            planetAlgorithm.GenerateVeins(false);
                            EVeinType type = (EVeinType)1;
                            foreach (VeinProto item in LDB.veins.dataArray)
                            {
                                long amount = planet.veinAmounts[(int)type];
                                if (type == EVeinType.Oil)
                                {
                                    Logger.LogInfo(type.ToString() + ": " + ((double)amount * VeinData.oilSpeedMultiplier).ToString("F5"));
                                }
                                else
                                {
                                    Logger.LogInfo(type.ToString() + ": " + amount.ToString());
                                }
                                type++;
                            }
                            planet.Free();
                        }
                    }
                }
            }
        }
    }
}

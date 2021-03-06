using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoPrologue
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class NoPrologue : BaseUnityPlugin
    {
        public const string pluginGuid = "dsp.dnightmare.noprologue";
        public const string pluginName = "No Prologue";
        public const string pluginVersion = "1.0.0";

        private bool cfgEnabled = true;

        new internal static BepInEx.Logging.ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(pluginName);

        void Awake()
        {
            cfgEnabled = Config.Bind<bool>("General", "enablePlugin", cfgEnabled, "enable/disable this plugin").Value;
            if (cfgEnabled == true)
            {
                var harmony = new Harmony(pluginGuid);
                harmony.PatchAll(typeof(NoPrologue));
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UIGalaxySelect), "EnterGame")]
        public static void EnterGamePostfix(ref GameDesc ___gameDesc)
        {
            DSPGame.StartGameSkipPrologue(___gameDesc);
        }
    }
}

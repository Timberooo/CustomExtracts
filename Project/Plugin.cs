using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace CustomExtracts
{
    [BepInPlugin("com.Timber.CustomExtracts", "CustomExtracts", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ConfigEntry<bool> ShowExtracts;
        internal static ConfigEntry<Color> DebugColor;



        private void Awake()
        {
            new OnGameStartPatch().Enable();
            new GameWorldOnDestroyPatch().Enable();

            ShowExtracts = Config.Bind("Debug",
                                       "Show extracts",
                                       false,
                                       new ConfigDescription("Shows the custom extraction zones"));

            DebugColor = Config.Bind("Debug",
                                     "Extract color",
                                     new Color(1f, 0f, 1f, 0.5f),
                                     new ConfigDescription("The color of extraction zones when visible"));

            ShowExtracts.SettingChanged += CustomExtractsManager.ShowExtracts_SettingChanged;
			DebugColor.SettingChanged   += CustomExtractsManager.DebugColor_SettingChanged;
        }
	}
}

using Aki.Reflection.Utils;
using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using EFT.UI;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomExtracts
{
	[BepInPlugin("com.Timber.CustomExtracts", "CustomExtracts", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		// BepInEx configs
		internal static ConfigEntry<Color>            extractColor;
		internal static ConfigEntry<Color>            currentExtractColor;
		internal static ConfigEntry<KeyboardShortcut> toggleExtractEditorPanel;



		private void Awake()
		{
			new OnGameStartPatch().Enable();
			new GameWorldOnDestroyPatch().Enable();

			extractColor = Config.Bind("", "Extract color", new Color(1f, 0f, 1f, 0.75f),
			                           new ConfigDescription("Extraction zone color", null,
			                           new ConfigurationManagerAttributes { IsAdvanced = true, Category = null, Order = 1 }));

			currentExtractColor = Config.Bind("", "Current extract color", new Color(0f, 1f, 0f, 0.75f),
			                                  new ConfigDescription("Current extraction zone color", null,
			                                  new ConfigurationManagerAttributes { IsAdvanced = true, Category = null, Order = 2 }));

			toggleExtractEditorPanel = Config.Bind("", "Extract editor key", new KeyboardShortcut(KeyCode.F5),
			                                       new ConfigDescription("The key that toggles the extract editor panel", null,
			                                       new ConfigurationManagerAttributes { IsAdvanced = true, Category = null, Order = 3}));

			extractColor.SettingChanged        += CustomExtractsManager.extractColor_SettingChanged;
			currentExtractColor.SettingChanged += CustomExtractsManager.currentExtractColor_SettingChanged;
		}
	}
}
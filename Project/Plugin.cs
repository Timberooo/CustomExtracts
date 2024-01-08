using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace CustomExtracts
{
	[BepInPlugin("com.Timber.CustomExtracts", "CustomExtracts", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		// No category
		internal static ConfigEntry<bool>  showExtracts;
		internal static ConfigEntry<Color> extractColor;
		internal static ConfigEntry<Color> currentExtractColor;

		// Current extract category
		internal static ConfigEntry<string> currentExtractName;



		private void Awake()
		{
			new OnGameStartPatch().Enable();
			new GameWorldOnDestroyPatch().Enable();

			showExtracts = Config.Bind("", "Show extracts", false,
			                           new ConfigDescription("Show custom extraction zones", null,
			                           new ConfigurationManagerAttributes { IsAdvanced = true, Category = null, Order = 0 }));

			extractColor = Config.Bind("", "Extract color", new Color(1f, 0f, 1f, 0.75f),
			                           new ConfigDescription("Extraction zone color", null,
			                           new ConfigurationManagerAttributes { IsAdvanced = true, Category = null, Order = 1 }));

			currentExtractColor = Config.Bind("", "Current extract color", new Color(0f, 1f, 0f, 0.75f),
			                           new ConfigDescription("Current extraction zone color", null,
			                           new ConfigurationManagerAttributes { IsAdvanced = true, Category = null, Order = 2 }));

			currentExtractName = Config.Bind("Current Extract", "Current extract", "",
			                           new ConfigDescription("The current extract being displayed and modified", null,
			                           new ConfigurationManagerAttributes { CustomDrawer = CurrentExtractNameDrawer, IsAdvanced = true, Order = 0, ReadOnly = true }));

			showExtracts.SettingChanged        += CustomExtractsManager.showExtracts_SettingChanged;
			extractColor.SettingChanged        += CustomExtractsManager.extractColor_SettingChanged;
			currentExtractColor.SettingChanged += CustomExtractsManager.currentExtractColor_SettingChanged;
		}



		private static void CurrentExtractNameDrawer(ConfigEntryBase entry)
		{
			// Left arrow: https://unicode.org/charts/PDF/U2B00.pdf
			if (GUILayout.Button("\u2B05", GUILayout.Width(45)))
				CustomExtractsManager.MoveToNextExtract();

			GUILayout.Label(currentExtractName.Value, GUILayout.ExpandWidth(true));

			// Right arrow: https://unicode.org/charts/PDF/U2B00.pdf
			if (GUILayout.Button("\u2B95", GUILayout.Width(45)))
				CustomExtractsManager.MoveToPreviousExtract();
		}
	}
}





//using BepInEx;
//using BepInEx.Configuration;
//using System.Collections.Generic;
//using UnityEngine;

//namespace CustomExtracts
//{
//    [BepInPlugin("com.Timber.CustomExtracts", "CustomExtracts", "1.0.0")]
//    public class Plugin : BaseUnityPlugin
//    {
//        internal static ConfigEntry<bool> ShowExtracts;
//        internal static ConfigEntry<Color> DebugColor;
//        internal static ConfigEntry<Color> SelectedDebugColor;

//        internal static ConfigEntry<string> SelectedExtractName;



//        private void Awake()
//        {
//            new OnGameStartPatch().Enable();
//            new GameWorldOnDestroyPatch().Enable();

//            ShowExtracts = Config.Bind("Debug",
//                                       "Show extracts",
//                                       false,
//                                       new ConfigDescription("Shows the custom extraction zones"));

//			DebugColor = Config.Bind("Debug",
//									 "Extract color",
//									 new Color(1f, 0f, 1f, 0.5f),
//									 new ConfigDescription("The color of extraction zones when visible"));

//			SelectedDebugColor = Config.Bind("Debug",
//									 "Current extract color",
//									 new Color(1f, 1f, 0f, 0.5f),
//									 new ConfigDescription("The color of the currently selected extraction zone when visible"));

//			SelectedExtractName = Config.Bind("Debug",
//                                             "Testing",
//                                             "",
//                                             new ConfigDescription("Desc", null, new ConfigurationManagerAttributes { CustomDrawer = StringListNavigationDrawer, ReadOnly = true }));

//            ShowExtracts.SettingChanged       += CustomExtractsManager.ShowExtracts_SettingChanged;
//			DebugColor.SettingChanged         += CustomExtractsManager.DebugColor_SettingChanged;
//			SelectedDebugColor.SettingChanged += CustomExtractsManager.SelectedDebugColor_SettingChanged;
//        }



//		static void StringListNavigationDrawer(ConfigEntryBase entry)
//        {
//            if (GUILayout.Button("<--", GUILayout.Width(45)))
//                CustomExtractsManager.MoveToPrevExtract();

//            GUILayout.TextField(SelectedExtractName.Value, GUILayout.ExpandWidth(true));

//            if (GUILayout.Button("-->", GUILayout.Width(45)))
//                CustomExtractsManager.MoveToNextExtract();
//        }
//	}
//}

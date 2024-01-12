using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace CustomExtracts
{
	[BepInPlugin("com.Timber.CustomExtracts", "CustomExtracts", "1.0.0")]
	internal class Plugin : BaseUnityPlugin
	{
		internal static ManualLogSource CustomExtractsLogger { get; } = BepInEx.Logging.Logger.CreateLogSource("CustomExtracts");

		internal static ConfigEntry<KeyboardShortcut> ToggleExtractEditor { get; private set; }
		internal static ConfigEntry<bool>             AlwaysShowExtracts { get; private set; }
		internal static ConfigEntry<Color>            ExtractColor { get; private set; }
		internal static ConfigEntry<Color>            CurrentExtractColor { get; private set; }
		internal static ConfigEntry<Color>            DisabledExtractColor { get; private set; }
		internal static ConfigEntry<Color>            DisabledCurrentExtractColor { get; private set; }



		private void Awake()
		{
			BepInEx.Logging.Logger.Sources.Add(CustomExtractsLogger);

			new GameWorldOnGameStartPatch().Enable();
			new GameWorldOnDestroyPatch().Enable();

			ToggleExtractEditor         = Config.Bind("Settings", "Editor key", new KeyboardShortcut(KeyCode.F5),
			                                          new ConfigDescription("The key to toggle the custom extract editor"));

			AlwaysShowExtracts          = Config.Bind("Settings", "Always show custom extracts", false,
			                                          new ConfigDescription("True to always show custom extracts; false to only show custom extracts when the editor is open"));

			ExtractColor                = Config.Bind("Settings", "Extract color", new Color(1f, 0f, 1f, 0.75f),
			                                          new ConfigDescription("The color of extracts when visible"));

			CurrentExtractColor         = Config.Bind("Settings", "Current extract color", new Color(0f, 1f, 0f, 0.75f),
			                                          new ConfigDescription("The color of the current extract when visible"));

			DisabledExtractColor        = Config.Bind("Settings", "Disabled extract color", new Color(0.5f, 0f, 0.5f, 0.75f),
			                                          new ConfigDescription("The color of disabled extracts when visible"));

			DisabledCurrentExtractColor = Config.Bind("Settings", "Disabled current extract color", new Color(0f, 0.5f, 0f, 0.75f),
			                                          new ConfigDescription("The color of the current extract when visible and disabled"));

			AlwaysShowExtracts.SettingChanged          += AlwaysShowExtracts_SettingChanged;
			ExtractColor.SettingChanged                += ExtractColor_SettingChanged;
			CurrentExtractColor.SettingChanged         += CurrentExtractColor_SettingChanged;
			DisabledExtractColor.SettingChanged        += DisabledExtractColor_SettingChanged;
			DisabledCurrentExtractColor.SettingChanged += DisabledCurrentExtractColor_SettingChanged;
		}



		private void OnDestroy()
		{
			BepInEx.Logging.Logger.Sources.Remove(CustomExtractsLogger);
		}



		private void AlwaysShowExtracts_SettingChanged(object sender, System.EventArgs e)
		{
			if (ExtractEditor.ShowEditor)
				return;

			if (Singleton<GameWorld>.Instantiated)
				Singleton<GameWorld>.Instance.gameObject.GetComponent<CustomExtractsManager>().ShowExtracts((bool)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
		}



		private void ExtractColor_SettingChanged(object sender, System.EventArgs e)
		{
			if (Singleton<GameWorld>.Instantiated)
				Singleton<GameWorld>.Instance.gameObject.GetComponent<CustomExtractsManager>().ChangeColor(CustomExtractsManager.ChangeColorForExtractState.General, (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
		}



		private void CurrentExtractColor_SettingChanged(object sender, System.EventArgs e)
		{
			if (Singleton<GameWorld>.Instantiated)
				Singleton<GameWorld>.Instance.gameObject.GetComponent<CustomExtractsManager>().ChangeColor(CustomExtractsManager.ChangeColorForExtractState.Current, (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
		}



		private void DisabledExtractColor_SettingChanged(object sender, System.EventArgs e)
		{
			if (Singleton<GameWorld>.Instantiated)
				Singleton<GameWorld>.Instance.gameObject.GetComponent<CustomExtractsManager>().ChangeColor(CustomExtractsManager.ChangeColorForExtractState.DisabledGeneral, (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
		}



		private void DisabledCurrentExtractColor_SettingChanged(object sender, System.EventArgs e)
		{
			if (Singleton<GameWorld>.Instantiated)
				Singleton<GameWorld>.Instance.gameObject.GetComponent<CustomExtractsManager>().ChangeColor(CustomExtractsManager.ChangeColorForExtractState.DisabledCurrent, (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
		}
	}
}

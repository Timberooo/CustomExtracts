using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomExtracts
{
	public static class CustomExtractsManager
	{
		private static List<GameObject> extracts = new List<GameObject>();
		private static int              currentExtractIndex = -1;



		public static void CreateExtract(Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time)
		{
			if (Singleton<GameWorld>.Instance == null)
				return;

			GameObject extract = GameObject.CreatePrimitive(PrimitiveType.Cube);

			extract.GetComponent<Collider>().isTrigger = true;

			ExtractTestComponent exfil = extract.AddComponent<ExtractTestComponent>();
			exfil.Duration = time;

			// All this crap is to make the debug meshes support transparency
			Renderer renderer = extract.GetComponent<Renderer>();
			renderer.material.SetOverrideTag("RenderType", "Transparent");
			renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			renderer.material.SetInt("_ZWrite", 0);
			renderer.material.DisableKeyword("_ALPHATEST_ON");
			renderer.material.EnableKeyword("_ALPHABLEND_ON");
			renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			renderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.material.color = Plugin.currentExtractColor.Value;
			renderer.enabled = Plugin.showExtracts.Value;

			extract.name = name;
			extract.layer = 13; // Copying SamSWAT's Fire Support extract implementation. No idea why this has to be set to this value
			extract.transform.position = position;
			extract.transform.eulerAngles = eulerAngles;
			extract.transform.localScale = size;

			extracts.Add(extract);

			// If current extract index is set, change extract color to
			// base color before updating the new current extract index
			if (currentExtractIndex > -1)
				extracts[currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.extractColor.Value;

			// New extracts are always added at the end of the extracts list
			currentExtractIndex = extracts.Count - 1;
			Plugin.currentExtractName.Value = name;
		}



		public static void DestroyAllExtracts()
		{
			extracts.ForEach(GameObject.Destroy);
			extracts.Clear();

			currentExtractIndex = -1;
			Plugin.currentExtractName.Value = (string)Plugin.currentExtractName.DefaultValue;
		}



		internal static void showExtracts_SettingChanged(object sender, EventArgs e)
		{
			extracts.ForEach(extract => extract.GetComponent<Renderer>().enabled = (bool)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
		}



		internal static void extractColor_SettingChanged(object sender, EventArgs e)
		{
			extracts.ForEach(extract =>
			{
				// Skip recoloring current extract so the current extract color isn't overwritten
				if (extract.name != Plugin.currentExtractName.Value)
					extract.GetComponent<Renderer>().material.color = (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue;
			});
		}



		internal static void currentExtractColor_SettingChanged(object sender, EventArgs e)
		{
			if (currentExtractIndex > -1)
				extracts[currentExtractIndex].GetComponent<Renderer>().material.color = (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue;
		}



		internal static void MoveToNextExtract()
		{
			if (currentExtractIndex <= -1)
				return;

			extracts[currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.extractColor.Value;

			// Checks if there is a next extract or if the
			// index should loop back to the first extract
			if (currentExtractIndex + 1 < extracts.Count)
				currentExtractIndex++;
			else
				currentExtractIndex = 0;

			extracts[currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.currentExtractColor.Value;
			Plugin.currentExtractName.Value = extracts[currentExtractIndex].name;
		}



		internal static void MoveToPreviousExtract()
		{
			if (currentExtractIndex <= -1)
				return;

			extracts[currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.extractColor.Value;

			// Checks if there is a previous extract or if
			// the index should loop back to the last extract
			if (currentExtractIndex - 1 >= 0)
				currentExtractIndex--;
			else
				currentExtractIndex = extracts.Count - 1;

			extracts[currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.currentExtractColor.Value;
			Plugin.currentExtractName.Value = extracts[currentExtractIndex].name;
		}
	}
}





//using BepInEx.Configuration;
//using Comfort.Common;
//using EFT;
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace CustomExtracts
//{
//	public static class CustomExtractsManager
//	{
//		private static List<GameObject> extracts = new();
//		private static int selectedExtractIndex = -1;



//		public static void CreateExtract(Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time)
//		{
//			if (Singleton<GameWorld>.Instance == null)
//				return;

//			GameObject extract = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			SetExtractProperties(extract, position, size, eulerAngles, name, time);

//			extracts.Add(extract);

//			if (selectedExtractIndex > -1)
//				extracts[selectedExtractIndex].GetComponent<Renderer>().material.color = Plugin.DebugColor.Value;

//			selectedExtractIndex = extracts.Count - 1;
//			Plugin.SelectedExtractName.Value = name;
//		}



//		public static void DestroyAllExtracts()
//		{
//			extracts.ForEach(extract => GameObject.Destroy(extract));
//			extracts.Clear();

//			selectedExtractIndex = -1;
//			Plugin.SelectedExtractName.Value = "";
//		}



//		internal static void ShowExtracts_SettingChanged(object sender, EventArgs e)
//		{
//			extracts.ForEach(extract => extract.GetComponent<Renderer>().enabled = (bool)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
//		}



//		internal static void DebugColor_SettingChanged(object sender, System.EventArgs e)
//		{
//			extracts.ForEach(extract => extract.GetComponent<Renderer>().material.color = (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
//        }



//		internal static void SelectedDebugColor_SettingChanged(object sender, System.EventArgs e)
//		{
//			if (selectedExtractIndex != -1)
//				extracts[selectedExtractIndex].GetComponent<Renderer>().material.color = (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue;
//		}



//		internal static void MoveToNextExtract()
//		{
//			if (selectedExtractIndex + 1 >= extracts.Count)
//				return;

//			Console.WriteLine($"MoveToNextExtract: selectedExtractIndex before (after): {selectedExtractIndex} ({selectedExtractIndex + 1})");
//			Console.WriteLine($"MoveToNextExtract: extracts.Count: {extracts.Count}");

//			extracts[selectedExtractIndex].GetComponent<Renderer>().material.color = Plugin.DebugColor.Value;

//			selectedExtractIndex++;

//			extracts[selectedExtractIndex].GetComponent<Renderer>().material.color = Plugin.SelectedDebugColor.Value;

//			Plugin.SelectedExtractName.Value = extracts[selectedExtractIndex].name;
//		}

//		internal static void MoveToPrevExtract()
//		{
//			if (selectedExtractIndex - 1 < 0)
//				return;

//			Console.WriteLine($"MoveToPrevExtract: selectedExtractIndex before (after): {selectedExtractIndex} ({selectedExtractIndex - 1})");
//			Console.WriteLine($"MoveToPrevExtract: extracts.Count: {extracts.Count}");

//			extracts[selectedExtractIndex].GetComponent<Renderer>().material.color = Plugin.DebugColor.Value;

//			selectedExtractIndex--;

//			extracts[selectedExtractIndex].GetComponent<Renderer>().material.color = Plugin.SelectedDebugColor.Value;

//			Plugin.SelectedExtractName.Value = extracts[selectedExtractIndex].name;
//		}



//		private static void SetExtractProperties(GameObject extract, Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time)
//		{
//			extract.GetComponent<Collider>().isTrigger = true;

//			// All this crap is to make the debug meshes transparent
//			Renderer renderer = extract.GetComponent<Renderer>();
//			renderer.material.SetOverrideTag("RenderType", "Transparent");
//			renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
//			renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//			renderer.material.SetInt("_ZWrite", 0);
//			renderer.material.DisableKeyword("_ALPHATEST_ON");
//			renderer.material.EnableKeyword("_ALPHABLEND_ON");
//			renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
//			renderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

//			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//			renderer.material.color = Plugin.SelectedDebugColor.Value;
//			renderer.enabled = Plugin.ShowExtracts.Value;

//			var test = extract.AddComponent<ExtractTestComponent>();
//			test.Duration = time;

//			extract.name = name;
//			extract.layer = 13; // Copying SamSWAT's Fire Support extract implementation. No idea why this has to be set to this value
//			extract.transform.position = position;
//			extract.transform.eulerAngles = eulerAngles;
//			extract.transform.localScale = size;
//		}
//	}
//}

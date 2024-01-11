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



		internal static GameObject CurrentExtract
		{
			get
			{
				if (currentExtractIndex > -1)
					return extracts[currentExtractIndex];
				else
					return null;
			}
		}



		public static void CreateExtract(Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time, bool enabled = true)
		{
			if (Singleton<GameWorld>.Instance == null)
				return;

			GameObject extract = GameObject.CreatePrimitive(PrimitiveType.Cube);

			extract.GetComponent<Collider>().isTrigger = true;
			extract.GetComponent<Collider>().enabled = enabled;

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
			renderer.enabled = ExtractEditor.ShowEditor;

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

			currentExtractIndex = extracts.Count - 1;
		}



		public static void DestroyAllExtracts()
		{
			extracts.ForEach(GameObject.Destroy);
			extracts.Clear();

			currentExtractIndex = -1;
		}



		public static void DeleteCurrentExtract()
		{
			GameObject.Destroy(extracts[currentExtractIndex]);
			extracts.RemoveAt(currentExtractIndex);

			if (extracts.Count <= 0)
				currentExtractIndex = -1;
			else
			{
				// Only need to update index when the last object was the current extract;
				// otherwise the next extract gets shifted to the index during removal

				if (currentExtractIndex >= extracts.Count)
					currentExtractIndex = extracts.Count - 1;

				extracts[currentExtractIndex].GetComponent<Renderer>().material.color = Color.green;
			}
		}



		internal static void IncrementCurrentExtract()
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
		}



		internal static void DecrementCurrentExtract()
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
		}



		internal static void ShowExtracts(bool show)
		{
			extracts.ForEach(extract => extract.GetComponent<Renderer>().enabled = show);
		}



		internal static void extractColor_SettingChanged(object sender, EventArgs e)
		{
			// Skip recoloring current extract so the
			// current extract color isn't overwritten

			for (int i = 0; i < extracts.Count; i++)
				if (i != currentExtractIndex)
					extracts[i].GetComponent<Renderer>().material.color = (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue;
		}



		internal static void currentExtractColor_SettingChanged(object sender, EventArgs e)
		{
			if (currentExtractIndex > -1)
				extracts[currentExtractIndex].GetComponent<Renderer>().material.color = (Color)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue;
		}
	}
}
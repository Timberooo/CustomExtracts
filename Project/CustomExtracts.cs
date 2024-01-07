using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CustomExtracts
{
	public static class CustomExtracts
	{
		private static List<GameObject> extracts = new();



		public class OnGameStartPatch : ModulePatch
		{
			protected override MethodBase GetTargetMethod()
			{
				return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
			}



			[PatchPostfix]
			public static void PatchPostfix()
			{
				Logger.LogDebug("OnGameStarted.PatchPostfix called");

				GameWorld gameWorld = Singleton<GameWorld>.Instance;

				if (gameWorld == null)
					return;

				// TODO: Load custom extracts from json

				Vector3 position = new Vector3(0f, 0f, 0f);
				Vector3 size = new Vector3(20f, 20f, 20f);
				Vector3 eulerAngles = new Vector3(0f, 0f, 0f);
				string name = "MyExtract";
				float time = 20f;

				Vector3 position2 = new Vector3(50f, 0f, -25f);
				Vector3 size2 = new Vector3(20f, 100f, 20f);
				Vector3 eulerAngles2 = new Vector3(0f, 45f, 0f);
				string name2 = "SecondExtract";
				float time2 = 10f;

				CreateCubeExtract(position, size, eulerAngles, name, time);
				CreateSphereExtract(position2, 40f, eulerAngles2, name2, time2);
			}
		}



		public class OnDestroyPatch : ModulePatch
		{
			protected override MethodBase GetTargetMethod()
			{
				return typeof(GameWorld).GetMethod("OnDestroy", BindingFlags.NonPublic | BindingFlags.Instance);
			}



			[PatchPostfix]
			public static void PatchPostfix()
			{
				Logger.LogDebug("OnDestroyPatch.PatchPostfix called");

				extracts.ForEach(GameObject.Destroy);
				extracts.Clear();
			}
		}



		private static void CreateCubeExtract(Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time)
		{
			if (Singleton<GameWorld>.Instance == null)
				return;

			GameObject extract = GameObject.CreatePrimitive(PrimitiveType.Cube);
			SetExtractProperties(extract, position, size, eulerAngles, name, time);

			extracts.Add(extract);
		}



		private static void CreateSphereExtract(Vector3 position, float radius, Vector3 eulerAngles, string name, float time)
		{
			if (Singleton<GameWorld>.Instance == null)
				return;

			GameObject extract = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			SetExtractProperties(extract, position, new Vector3(radius, radius, radius), eulerAngles, name, time);

			extracts.Add(extract);
		}



		private static void SetExtractProperties(GameObject extract, Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time)
		{
			extract.name = name;
			extract.layer = 13; // Copying SamSWAT's Fire Support extract implementation. No idea why this has to be set to this value
			extract.transform.position = position;
			extract.transform.eulerAngles = eulerAngles;
			extract.transform.localScale = size;

			extract.GetComponent<Collider>().isTrigger = true;

			// All this crap is to make the debug meshes transparent
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
			renderer.material.color = new Color(1f, 1f, 0f, 0.75f);
			renderer.enabled = true;

			var test = extract.AddComponent<ExtractTestComponent>();
			test.Duration = time;
		}
	}
}

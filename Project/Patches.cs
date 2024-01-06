using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CustomExtracts
{
	public static class Patches
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

				CreateExtract(position, size, eulerAngles, name, time);

				Vector3 position2 = new Vector3(50f, 0f, -25f);
				Vector3 size2 = new Vector3(20f, 100f, 20f);
				Vector3 eulerAngles2 = new Vector3(0f, 45f, 0f);
				string name2 = "SecondExtract";
				float time2 = 10f;

				CreateExtract(position2, size2, eulerAngles2, name2, time2);
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



		public static void CreateExtract(Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time)
		{
			Console.WriteLine("CreateExtract called");

			GameWorld gameWorld = Singleton<GameWorld>.Instance;

			if (gameWorld == null)
				return;

			GameObject extract = new();
			extract.name = name;
			extract.layer = 13;
			extract.transform.position = position;
			extract.transform.eulerAngles = eulerAngles;
			extract.transform.localScale = size;

			var test = extract.AddComponent<ExtractTestComponent>();
			test.Duration = time;

			BoxCollider collider = extract.AddComponent<BoxCollider>();
			collider.isTrigger = true;

			MeshFilter debugMeshFilter = extract.AddComponent<MeshFilter>();
			Renderer debugRenderer = extract.AddComponent<Renderer>(); // Unity gets upset about this but it seems transparency doesn't work without it
			MeshRenderer debugMeshRenderer = extract.AddComponent<MeshRenderer>();

			GameObject tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
			debugMeshFilter.mesh = tempPrimitive.GetComponent<MeshFilter>().mesh;
			debugRenderer = tempPrimitive.GetComponent<Renderer>();
			debugMeshRenderer.material = tempPrimitive.GetComponent<MeshRenderer>().material;
			GameObject.Destroy(tempPrimitive);

			debugRenderer.material.SetOverrideTag("RenderType", "Transparent");
			debugRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			debugRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			debugRenderer.material.SetInt("_ZWrite", 0);
			debugRenderer.material.DisableKeyword("_ALPHATEST_ON");
			debugRenderer.material.EnableKeyword("_ALPHABLEND_ON");
			debugRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			debugRenderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

			debugRenderer.material.color = new Color(1f, 1f, 0f, 0.5f);
			debugMeshRenderer.enabled = true;

			extracts.Add(extract);

			Console.WriteLine("New extract created and added to extracts list");
		}
	}
}

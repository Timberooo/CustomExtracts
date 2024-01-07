using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CustomExtracts
{
	public static class CustomExtracts
	{
		private static List<GameObject> extracts = new();



		private enum Shape
		{
			Cube,
			Sphere,
			Capsule
		}



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
				Shape shape = Shape.Cube;
				string name = "MyExtract";
				float time = 20f;

				CreateExtract(position, size, eulerAngles, shape, name, time);

				Vector3 position2 = new Vector3(50f, 0f, -25f);
				Vector3 size2 = new Vector3(20f, 100f, 20f);
				Vector3 eulerAngles2 = new Vector3(0f, 45f, 0f);
				Shape shape2 = Shape.Sphere;
				string name2 = "SecondExtract";
				float time2 = 10f;

				CreateExtract(position2, size2, eulerAngles2, shape2, name2, time2);
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



		private static void CreateExtract(Vector3 position, Vector3 size, Vector3 eulerAngles, Shape shape, string name, float time)
		{
			Console.WriteLine("CreateExtract called");

			GameWorld gameWorld = Singleton<GameWorld>.Instance;

			if (gameWorld == null)
				return;

			GameObject extract = null;

			switch (shape)
			{
				case Shape.Cube:
				{
					extract = GameObject.CreatePrimitive(PrimitiveType.Cube);
					break;
				}
				case Shape.Sphere:
				{
					extract = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					break;
				}
				case Shape.Capsule:
				{
					extract = GameObject.CreatePrimitive(PrimitiveType.Capsule);
					break;
				}
			}

			if (extract == null) // Error, shouldn't ever happen
				return;

			extract.name = name;
			extract.layer = 13; // Copying SamSWAT's Fire Support extract implementation. No idea why this has to be set to this value
			extract.transform.position = position;
			extract.transform.eulerAngles = eulerAngles;
			extract.transform.localScale = size; // This should be moved into the shape switch-case

			extract.GetComponent<Collider>().isTrigger = true;

			// All this crap is to make the debug meshes transparent
			var renderer = extract.GetComponent<Renderer>();
			renderer.material.SetOverrideTag("RenderType", "Transparent");
			renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			renderer.material.SetInt("_ZWrite", 0);
			renderer.material.DisableKeyword("_ALPHATEST_ON");
			renderer.material.EnableKeyword("_ALPHABLEND_ON");
			renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			renderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

			renderer.material.color = new Color(1f, 1f, 0f, 0.5f);
			renderer.enabled = true;

			var test = extract.AddComponent<ExtractTestComponent>();
			test.Duration = time;

			extracts.Add(extract);

			Console.WriteLine($"Extract \"{extract.name}\" created and added to list of custom extracts");
		}



		//private static void CreateExtract(Vector3 position, Vector3 size, Vector3 eulerAngles, Shape shape, string name, float time)
		//{
		//	Console.WriteLine("CreateExtract called");

		//	GameWorld gameWorld = Singleton<GameWorld>.Instance;

		//	if (gameWorld == null)
		//		return;

		//	GameObject extract = new();
		//	extract.name = name;
		//	extract.layer = 13; // Copying SamSWAT's Fire Support extract implementation. No idea why this has to be set to this value
		//	extract.transform.position = position;
		//	extract.transform.eulerAngles = eulerAngles;
		//	extract.transform.localScale = size;

		//	var test = extract.AddComponent<ExtractTestComponent>();
		//	test.Duration = time;

		//	switch (shape)
		//	{
		//		case Shape.Cube:
		//			extract.AddComponent<BoxCollider>(); break;
		//		case Shape.Sphere:
		//			extract.AddComponent<SphereCollider>(); break;
		//		case Shape.Capsule:
		//			extract.AddComponent<CapsuleCollider>(); break;
		//	}

		//	extract.GetComponent<Collider>().isTrigger = true;

		//	AddDebugMesh(extract, shape);

		//	extracts.Add(extract);

		//	Console.WriteLine($"New extract \"{extract.name}\" created and added to extracts list");
		//}



		//private static void AddDebugMesh(GameObject extract, Shape shape)
		//{
		//	MeshFilter meshFilter = extract.AddComponent<MeshFilter>();
		//	Renderer renderer = extract.AddComponent<Renderer>(); // Unity gets upset about this but it seems transparency doesn't work without it
		//	MeshRenderer meshRenderer = extract.AddComponent<MeshRenderer>();

		//	GameObject tempPrimitive = null;

		//	switch (shape)
		//	{
		//		case Shape.Cube:
		//			tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube); break;
		//		case Shape.Sphere:
		//			tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere); break;
		//		case Shape.Capsule:
		//			tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Capsule); break;
		//	}

		//	meshFilter.mesh = tempPrimitive.GetComponent<MeshFilter>().mesh;
		//	renderer = tempPrimitive.GetComponent<Renderer>();
		//	meshRenderer.material = tempPrimitive.GetComponent<MeshRenderer>().material;

		//	if (tempPrimitive != null)
		//		GameObject.Destroy(tempPrimitive);

		//	renderer.material.SetOverrideTag("RenderType", "Transparent");
		//	renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		//	renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		//	renderer.material.SetInt("_ZWrite", 0);
		//	renderer.material.DisableKeyword("_ALPHATEST_ON");
		//	renderer.material.EnableKeyword("_ALPHABLEND_ON");
		//	renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		//	renderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

		//	renderer.material.color = new Color(1f, 1f, 0f, 0.5f);
		//	meshRenderer.enabled = true;
		//}
	}
}

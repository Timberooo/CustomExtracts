using Comfort.Common;
using EFT;
using System.Collections.Generic;
using UnityEngine;

namespace CustomExtracts
{
	public static class CustomExtractsManager
	{
		private static List<GameObject> extracts = new();



		public static void CreateExtract(Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time)
		{
			if (Singleton<GameWorld>.Instance == null)
				return;

			GameObject extract = GameObject.CreatePrimitive(PrimitiveType.Cube);
			SetExtractProperties(extract, position, size, eulerAngles, name, time);

			extracts.Add(extract);
		}



		public static void DestroyAllExtracts()
		{
			extracts.ForEach(extract => Object.Destroy(extract));
			extracts.Clear();
		}



		private static void SetExtractProperties(GameObject extract, Vector3 position, Vector3 size, Vector3 eulerAngles, string name, float time)
		{
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
			renderer.material.color = new Color(1f, 0f, 1f, 0.5f);
			renderer.enabled = true;

			var test = extract.AddComponent<ExtractTestComponent>();
			test.Duration = time;

			extract.name = name;
			extract.layer = 13; // Copying SamSWAT's Fire Support extract implementation. No idea why this has to be set to this value
			extract.transform.position = position;
			extract.transform.eulerAngles = eulerAngles;
			extract.transform.localScale = size;
		}
	}
}

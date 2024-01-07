using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using System.Reflection;
using UnityEngine;

namespace CustomExtracts
{
	public class OnGameStartPatch : ModulePatch
	{
		protected override MethodBase GetTargetMethod()
		{
			return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
		}



		[PatchPostfix]
		public static void PatchPostfix()
		{
			Logger.LogDebug("OnGameStartedPatch.PatchPostfix called");

			if (Singleton<GameWorld>.Instance == null)
				return;

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

			CustomExtracts.CreateExtract(position, size, eulerAngles, name, time);
			//CustomExtracts.CreateSphereExtract(position2, 40f, eulerAngles2, name2, time2);
		}
	}
}

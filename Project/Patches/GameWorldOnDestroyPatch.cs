using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using System.Reflection;

namespace CustomExtracts
{
	public class GameWorldOnDestroyPatch : ModulePatch
	{
		protected override MethodBase GetTargetMethod()
		{
			return typeof(GameWorld).GetMethod("OnDestroy", BindingFlags.NonPublic | BindingFlags.Instance);
		}



		[PatchPostfix]
		public static void PatchPostfix()
		{
			Logger.LogDebug("GameWorldOnDestroyPatch.PatchPostfix called");

			if (Singleton<GameWorld>.Instantiated)
				Singleton<GameWorld>.Instance.gameObject.GetComponent<CustomExtractsManager>().DestroyAllExtracts();
		}
	}
}

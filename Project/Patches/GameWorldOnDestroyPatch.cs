using Aki.Reflection.Patching;
using EFT;
using System.Reflection;

namespace CustomExtracts.Patches
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

			CustomExtractsManager.DestroyAllExtracts();
		}
	}
}

using BepInEx;

namespace CustomExtracts
{
    [BepInPlugin("com.Timber.CustomExtracts", "CustomExtracts", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new Patches.OnGameStartPatch().Enable();
            new Patches.OnDestroyPatch().Enable();
        }
    }
}

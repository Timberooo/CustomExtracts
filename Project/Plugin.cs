using BepInEx;

namespace CustomExtracts
{
    [BepInPlugin("com.Timber.CustomExtracts", "CustomExtracts", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new CustomExtracts.OnGameStartPatch().Enable();
            new CustomExtracts.OnDestroyPatch().Enable();
        }
    }
}

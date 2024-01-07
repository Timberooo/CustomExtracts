using BepInEx;
using CustomExtracts;

namespace CustomExtracts
{
    [BepInPlugin("com.Timber.CustomExtracts", "CustomExtracts", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new OnGameStartPatch().Enable();
            new GameWorldOnDestroyPatch().Enable();
        }
    }
}

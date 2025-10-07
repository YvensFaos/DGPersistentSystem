using UnityEngine;

namespace DGPersistentSystem
{
    public class PlayerPrefManager
    {
        public static void SaveRegistryOnPlayerPref(string registryName, SaveRegistry registry)
        {
            PlayerPrefs.SetString(registryName, registry.SaveRegistryToJson());
        }
    }
}
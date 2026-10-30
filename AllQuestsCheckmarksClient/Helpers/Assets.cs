using UnityEngine;

namespace AllQuestsCheckmarks.Helpers
{
    internal static class Assets
    {
        public static Sprite? Checkmark;
        private static AssetBundle? _bundle;

        public static void LoadAssets()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(Plugin.modPath + "/AllQuestsCheckmarksAssets");
            _bundle = bundle;

            if (bundle is null)
            {
                Plugin.LogSource?.LogError("Failed to load asset bundle!");
                return;
            }

            // A managed IL2CPP wrapper alone is not a Unity asset reference.
            // Keep these assets through the game's startup UnloadUnusedAssets.
            bundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            foreach (var name in bundle.GetAllAssetNames())
            {
                Plugin.LogSource?.LogInfo("Checkmark bundle asset: " + name);
                var assets = bundle.LoadAssetWithSubAssets(name, Il2CppInterop.Runtime.Il2CppType.Of<Sprite>());
                foreach (var asset in assets)
                {
                    Checkmark = asset.TryCast<Sprite>();
                    if (Checkmark != null)
                    {
                        Checkmark.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                        Checkmark.texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                        break;
                    }
                }
                if (Checkmark != null) break;
            }

            if (Checkmark != null) Plugin.LogSource?.LogInfo("Checkmark sprite loaded");
            else Plugin.LogSource?.LogError("Checkmark bundle contains no loadable sprite");
        }
    }

}

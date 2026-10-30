using AllQuestsCheckmarks.Helpers;
using EFT;
using HarmonyLib;
using SPTushonka.Reflection.Patching;
using System.Reflection;

namespace AllQuestsCheckmarks.Patches
{
    internal class ProfileSelectionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(EftClientBackendSession), nameof(EftClientBackendSession.SetMainProfile));
        }

        [PatchPostfix]
        static void Postfix(Il2CppSystem.Threading.Tasks.Task<JsonType.SelectProfileResponse> __result)
        {
            Plugin.LogDebug("Profile selected");
            Plugin.PendingSelection = __result;
        }
    }
}

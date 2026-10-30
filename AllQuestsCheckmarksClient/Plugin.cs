using AllQuestsCheckmarks.Helpers;
using AllQuestsCheckmarks.Patches;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using System.IO;
using System.Reflection;

namespace AllQuestsCheckmarks
{
    [
        BepInPlugin("com.zgfuedkx.allquestscheckmarks", "ZGFueDkx-AllQuestsCheckmarks", "1.4.0"),
        BepInDependency("com.sptushonka.core", "5.0.0"),
        BepInDependency("com.fika.core", BepInDependency.DependencyFlags.SoftDependency),
    ]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource? LogSource;
        internal static Il2CppSystem.Threading.Tasks.Task<JsonType.SelectProfileResponse>? PendingSelection;
        internal static void CheckProfileSelection()
        {
            var task = PendingSelection;
            if (task == null || !task.IsCompleted) return;
            PendingSelection = null;
            if (task.IsFaulted || task.IsCanceled) return;
            try { QuestsData.LoadData(); }
            catch (System.Exception error) { LogSource?.LogError("Quest data refresh failed: " + error); }
        }
        public static bool isFikaInstalled = false;
        public static string? modPath;

        public override void Load()
        {
            LogSource = Log;

            modPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Plugin)).Location);
            modPath.Replace("\\", "/");

            isFikaInstalled = IL2CPPChainloader.Instance.Plugins.ContainsKey("com.fika.core");

            AddComponent<ProfileSelectionPump>();
            Settings.Init(Config);
            Helpers.Assets.LoadAssets();

            if (isFikaInstalled)
            {
                FikaBridge.Init();
            }
            else
            {
                new LocalGameStartPatch().Enable();
            }

            new QuestClassPatch().Enable();
            new ProfileSelectionPatch().Enable();
            new QuestItemViewPanelPatch().Enable();
            new UpdateApplicationLanguagePatch().Enable();

            LogSource.LogInfo($"AllQuestCheckmarks by ZGFueDkx version 1.4.0 (local SPT 5.0 port) started");
        }

        public static void LogDebug(string msg)
        {
            if (Settings.ShowDebug!.Value)
            {
                LogSource?.LogDebug(msg);
            }
        }
    }
}

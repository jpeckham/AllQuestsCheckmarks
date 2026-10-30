using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using System.Collections;
using System.Reflection;
using UnityEngine;
namespace AllQuestsCheckmarksRuntimeChecks;
[BepInPlugin("com.jpeckham.checkmarks.runtimechecks", "Checkmarks Runtime Checks", "1.0.0")]
[BepInDependency("com.zgfuedkx.allquestscheckmarks", "1.4.0")]
public class RuntimeChecks : BasePlugin
{
    internal static RuntimeChecks Instance;
    private bool ran;
    private readonly DateTime deadline = DateTime.UtcNow.AddSeconds(15);
    public override void Load() { Instance=this; AddComponent<CheckPump>(); }
    internal void Tick()
    {
        if(ran) return;
        try {
            var assembly=typeof(AllQuestsCheckmarks.Plugin).Assembly;
            if(DateTime.UtcNow<deadline) return;
            var dataType=assembly.GetType("AllQuestsCheckmarks.Helpers.QuestsData")!;
            dataType.GetMethod("LoadData")!.Invoke(null,null);
            var cache=(IDictionary)dataType.GetField("QuestItemsByItemId")!.GetValue(null)!;
            ran=true;
            if(cache.Count==0) throw new Exception("Explicit read-only quest data refresh did not populate item cache");
            var sprite=assembly.GetType("AllQuestsCheckmarks.Helpers.Assets")!.GetField("Checkmark")!.GetValue(null) as Sprite;
            if(sprite==null) throw new Exception("Checkmark asset not loaded");
            foreach(EFT.MongoID key in cache.Keys) {
                if(!cache.Contains(new EFT.MongoID(key.ToString()))) throw new Exception("Native MongoID value equality failed");
            }
            var color=new Color(1,0.5f,0,0.25f);
            var serialized=TomlTypeConverter.ConvertToString(color,typeof(Color));
            var restored=(Color)TomlTypeConverter.ConvertToValue(serialized,typeof(Color));
            if(Math.Abs(restored.a-color.a)>1f/255 || Math.Abs(restored.g-color.g)>1f/255) throw new Exception("Color serialization lost channels");
            Log.LogInfo($"PASS: {cache.Count} cached item templates after explicit read-only refresh, native ID equality, checkmark sprite, RGBA config roundtrip and injected Update");
        } catch(Exception e) { ran=true; Log.LogError("FAIL: "+e); }
    }
}
public sealed class CheckPump : MonoBehaviour
{
    public CheckPump(IntPtr pointer):base(pointer){}
    public void Update()=>RuntimeChecks.Instance.Tick();
}

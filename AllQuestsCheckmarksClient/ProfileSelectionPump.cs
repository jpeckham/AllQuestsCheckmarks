using System;
using UnityEngine;
namespace AllQuestsCheckmarks;
public sealed class ProfileSelectionPump : MonoBehaviour
{
    public ProfileSelectionPump(IntPtr pointer) : base(pointer) { }
    public void Update() => Plugin.CheckProfileSelection();
}

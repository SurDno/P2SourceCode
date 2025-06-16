// Decompiled with JetBrains decompiler
// Type: SRDebugger.Internal.SRDebuggerUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace SRDebugger.Internal
{
  public static class SRDebuggerUtil
  {
    public static bool EnsureEventSystemExists()
    {
      if ((Object) EventSystem.current != (Object) null)
        return false;
      EventSystem objectOfType = Object.FindObjectOfType<EventSystem>();
      if ((Object) objectOfType != (Object) null && objectOfType.gameObject.activeSelf && objectOfType.enabled)
        return false;
      Debug.LogWarning((object) "[SRDebugger] No EventSystem found in scene - creating a default one.");
      SRDebuggerUtil.CreateDefaultEventSystem();
      return true;
    }

    public static void CreateDefaultEventSystem()
    {
      GameObject gameObject = new GameObject("EventSystem");
      gameObject.AddComponent<EventSystem>();
      gameObject.AddComponent<StandaloneInputModule>();
    }
  }
}

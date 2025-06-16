// Decompiled with JetBrains decompiler
// Type: SRDebugger.Profiler.ProfilerLateUpdateListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace SRDebugger.Profiler
{
  public class ProfilerLateUpdateListener : MonoBehaviour
  {
    public Action OnLateUpdate;

    private void LateUpdate()
    {
      Action onLateUpdate = this.OnLateUpdate;
      if (onLateUpdate == null)
        return;
      onLateUpdate();
    }
  }
}

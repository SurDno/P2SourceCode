// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Profiler.ProfilerCollectLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRF;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace SRDebugger.UI.Profiler
{
  public class ProfilerCollectLabel : SRMonoBehaviour
  {
    [SerializeField]
    private Text _text;
    private float updateFrequency = 1f;
    private float nextUpdate;

    private void Update()
    {
      if ((double) Time.realtimeSinceStartup <= (double) this.nextUpdate)
        return;
      this.Refresh();
    }

    private void Refresh()
    {
      this.nextUpdate = Time.realtimeSinceStartup + this.updateFrequency;
      this._text.text = "Collect : " + (object) GC.CollectionCount(0);
    }
  }
}

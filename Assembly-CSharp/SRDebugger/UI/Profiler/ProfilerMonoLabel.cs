// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Profiler.ProfilerMonoLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services;
using SRF;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace SRDebugger.UI.Profiler
{
  public class ProfilerMonoLabel : SRMonoBehaviour
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
      long monoHeapSizeLong = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
      this._text.text = "Mono : " + OptimizationUtility.GetMemoryText(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong()) + " / " + OptimizationUtility.GetMemoryText(monoHeapSizeLong);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Profiler.ProfilerFPSLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRDebugger.Services;
using SRF;
using SRF.Service;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace SRDebugger.UI.Profiler
{
  public class ProfilerFPSLabel : SRMonoBehaviour
  {
    [SerializeField]
    private Text _text;
    private IProfilerService _profilerService;
    private float updateFrequency = 1f;
    private float nextUpdate;
    private int count;

    private void Update()
    {
      ++this.count;
      if ((double) Time.realtimeSinceStartup <= (double) this.nextUpdate)
        return;
      this.Refresh();
    }

    private void Refresh()
    {
      this._text.text = "FPS: " + (object) this.count;
      this.nextUpdate = Time.realtimeSinceStartup + this.updateFrequency;
      this.count = 0;
    }

    protected void Awake()
    {
      this._profilerService = SRServiceManager.GetService<IProfilerService>();
    }
  }
}

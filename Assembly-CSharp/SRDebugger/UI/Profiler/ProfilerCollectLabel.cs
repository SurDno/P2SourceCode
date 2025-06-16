using SRF;
using System;
using UnityEngine;
using UnityEngine.UI;

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

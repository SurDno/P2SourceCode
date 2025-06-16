// Decompiled with JetBrains decompiler
// Type: Rain.Bokeh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Rain
{
  [RequireComponent(typeof (ParticleSystem))]
  public class Bokeh : MonoBehaviour
  {
    public float maxRate = 10f;
    private ParticleSystem _system;

    private ParticleSystem system
    {
      get
      {
        if ((Object) this._system == (Object) null)
          this._system = this.GetComponent<ParticleSystem>();
        return this._system;
      }
    }

    private void Update()
    {
      RainManager instance = RainManager.Instance;
      float constant;
      if ((Object) instance != (Object) null)
      {
        float actualRainIntensity = instance.actualRainIntensity;
        Vector3 normalized = new Vector3(-instance.actualWindVector.x, 1f, -instance.actualWindVector.y).normalized;
        float num = Mathf.Clamp01((float) ((double) Vector3.Dot(this.transform.forward, normalized) * 0.89999997615814209 + 0.10000000149011612));
        constant = (double) num > 0.0 && !Physics.Raycast(this.transform.position, normalized, 50f) ? actualRainIntensity * (this.maxRate * num) : 0.0f;
      }
      else
        constant = 0.0f;
      this.system.emission.rateOverTime = new ParticleSystem.MinMaxCurve(constant);
    }
  }
}

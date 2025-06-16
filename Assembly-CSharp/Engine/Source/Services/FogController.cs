using Engine.Common;
using Engine.Impl.Weather.Element;
using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (FogController)})]
  public class FogController : IInitialisable
  {
    private GlobalFog engineFog;

    public event Action<float> DensityChangedEvent;

    public void Initialise() => this.engineFog = UnityEngine.Object.FindObjectOfType<GlobalFog>();

    public void Terminate()
    {
    }

    public void CopyTo(Fog fog)
    {
      if ((UnityEngine.Object) this.engineFog == (UnityEngine.Object) null)
        return;
      fog.Density = RenderSettings.fogDensity;
      fog.StartDistance = this.engineFog.startDistance;
      fog.Height = this.engineFog.height;
    }

    public void CopyFrom(Fog fog)
    {
      if ((UnityEngine.Object) this.engineFog == (UnityEngine.Object) null)
        return;
      if ((double) RenderSettings.fogDensity != (double) fog.Density)
      {
        RenderSettings.fogDensity = fog.Density;
        Action<float> densityChangedEvent = this.DensityChangedEvent;
        if (densityChangedEvent != null)
          densityChangedEvent(fog.Density);
      }
      this.engineFog.height = fog.Height;
      this.engineFog.startDistance = fog.StartDistance;
      Shader.SetGlobalFloat("_FogStartDistance", fog.StartDistance);
    }
  }
}

using System;
using Engine.Common;
using Engine.Impl.Weather.Element;
using UnityStandardAssets.ImageEffects;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (FogController))]
  public class FogController : IInitialisable
  {
    private GlobalFog engineFog;

    public event Action<float> DensityChangedEvent;

    public void Initialise() => engineFog = UnityEngine.Object.FindObjectOfType<GlobalFog>();

    public void Terminate()
    {
    }

    public void CopyTo(Fog fog)
    {
      if ((UnityEngine.Object) engineFog == (UnityEngine.Object) null)
        return;
      fog.Density = RenderSettings.fogDensity;
      fog.StartDistance = engineFog.startDistance;
      fog.Height = engineFog.height;
    }

    public void CopyFrom(Fog fog)
    {
      if ((UnityEngine.Object) engineFog == (UnityEngine.Object) null)
        return;
      if ((double) RenderSettings.fogDensity != fog.Density)
      {
        RenderSettings.fogDensity = fog.Density;
        Action<float> densityChangedEvent = DensityChangedEvent;
        if (densityChangedEvent != null)
          densityChangedEvent(fog.Density);
      }
      engineFog.height = fog.Height;
      engineFog.startDistance = fog.StartDistance;
      Shader.SetGlobalFloat("_FogStartDistance", fog.StartDistance);
    }
  }
}

using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

[RequireComponent(typeof (VolumetricLight))]
public class VolumetricLightDensity : EngineDependent
{
  [SerializeField]
  private float referenceDensity = 0.05f;
  private VolumetricLight volumetricLight;
  private float baseExtinction;
  private float baseScattering;
  [FromLocator]
  private FogController fogController;

  private void ApplyDensity(float density)
  {
    if (referenceDensity == 0.0)
      return;
    if (volumetricLight == null)
    {
      volumetricLight = GetComponent<VolumetricLight>();
      baseExtinction = volumetricLight.ExtinctionCoef;
      baseScattering = volumetricLight.ScatteringCoef;
    }
    float num = density / referenceDensity;
    volumetricLight.ExtinctionCoef = baseExtinction * num;
    volumetricLight.ScatteringCoef = baseScattering * num;
  }

  protected override void OnConnectToEngine()
  {
    ApplyDensity(RenderSettings.fogDensity);
    fogController.DensityChangedEvent += ApplyDensity;
  }

  protected override void OnDisconnectFromEngine()
  {
    fogController.DensityChangedEvent -= ApplyDensity;
  }
}

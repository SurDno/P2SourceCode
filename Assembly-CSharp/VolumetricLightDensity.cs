// Decompiled with JetBrains decompiler
// Type: VolumetricLightDensity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using System;
using UnityEngine;

#nullable disable
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
    if ((double) this.referenceDensity == 0.0)
      return;
    if ((UnityEngine.Object) this.volumetricLight == (UnityEngine.Object) null)
    {
      this.volumetricLight = this.GetComponent<VolumetricLight>();
      this.baseExtinction = this.volumetricLight.ExtinctionCoef;
      this.baseScattering = this.volumetricLight.ScatteringCoef;
    }
    float num = density / this.referenceDensity;
    this.volumetricLight.ExtinctionCoef = this.baseExtinction * num;
    this.volumetricLight.ScatteringCoef = this.baseScattering * num;
  }

  protected override void OnConnectToEngine()
  {
    this.ApplyDensity(RenderSettings.fogDensity);
    this.fogController.DensityChangedEvent += new Action<float>(this.ApplyDensity);
  }

  protected override void OnDisconnectFromEngine()
  {
    this.fogController.DensityChangedEvent -= new Action<float>(this.ApplyDensity);
  }
}

// Decompiled with JetBrains decompiler
// Type: LightServiceObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

#nullable disable
public class LightServiceObject : MonoBehaviour
{
  [SerializeField]
  private float visibilityRadius = 5f;

  public float VisibilityRadius => this.visibilityRadius;

  private void OnEnable() => ServiceLocator.GetService<LightService>()?.RegisterLight(this, true);

  private void OnDisable() => ServiceLocator.GetService<LightService>()?.RegisterLight(this, false);
}

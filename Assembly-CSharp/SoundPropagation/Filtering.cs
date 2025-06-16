// Decompiled with JetBrains decompiler
// Type: SoundPropagation.Filtering
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  [Serializable]
  public struct Filtering
  {
    [Tooltip("High frequency loss")]
    public float Occlusion;

    public void AddFiltering(Filtering filteringPerMeter, float distance)
    {
      this.Occlusion += filteringPerMeter.Occlusion;
    }

    public void AddOcclusion(float occlusion) => this.Occlusion += occlusion;

    public float Loss => this.Occlusion;
  }
}

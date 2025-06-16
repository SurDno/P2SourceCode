// Decompiled with JetBrains decompiler
// Type: SoundPropagation.SPCellProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  [CreateAssetMenu(fileName = "New Cell Profile", menuName = "Scriptable Objects/Sound Propagation Cell Profile")]
  public class SPCellProfile : ScriptableObject
  {
    public Filtering FilteringPerMeter;

    public float OcclusionPerMeter => this.FilteringPerMeter.Occlusion;
  }
}

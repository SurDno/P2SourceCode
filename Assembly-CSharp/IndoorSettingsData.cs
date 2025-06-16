// Decompiled with JetBrains decompiler
// Type: IndoorSettingsData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Regions;
using UnityEngine;

#nullable disable
public class IndoorSettingsData : ScriptableObjectInstance<IndoorSettingsData>
{
  [SerializeField]
  private BuildingEnum[] isolatedIndoors;

  public bool IsIndoorIsolated(BuildingEnum building)
  {
    if (this.isolatedIndoors == null)
      return false;
    for (int index = 0; index < this.isolatedIndoors.Length; ++index)
    {
      if (this.isolatedIndoors[index] == building)
        return true;
    }
    return false;
  }
}

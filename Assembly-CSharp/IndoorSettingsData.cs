using Engine.Common.Components.Regions;
using UnityEngine;

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

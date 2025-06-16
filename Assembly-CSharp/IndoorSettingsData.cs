using Engine.Common.Components.Regions;

public class IndoorSettingsData : ScriptableObjectInstance<IndoorSettingsData>
{
  [SerializeField]
  private BuildingEnum[] isolatedIndoors;

  public bool IsIndoorIsolated(BuildingEnum building)
  {
    if (isolatedIndoors == null)
      return false;
    for (int index = 0; index < isolatedIndoors.Length; ++index)
    {
      if (isolatedIndoors[index] == building)
        return true;
    }
    return false;
  }
}

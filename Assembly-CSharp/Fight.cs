using UnityEngine;

public static class Fight
{
  private static FightDescription description;

  public static FightDescription Description
  {
    get
    {
      if ((Object) Fight.description == (Object) null)
        Fight.description = ScriptableObjectInstance<FightSettingsData>.Instance.Description;
      return Fight.description;
    }
  }
}

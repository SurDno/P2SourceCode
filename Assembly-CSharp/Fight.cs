public static class Fight
{
  private static FightDescription description;

  public static FightDescription Description
  {
    get
    {
      if ((Object) description == (Object) null)
        description = ScriptableObjectInstance<FightSettingsData>.Instance.Description;
      return description;
    }
  }
}

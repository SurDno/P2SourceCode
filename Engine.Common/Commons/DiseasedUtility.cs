namespace Engine.Common.Commons
{
  public static class DiseasedUtility
  {
    public static DiseasedStateEnum GetStateByLevel(int diseaseLevel)
    {
      if (diseaseLevel < 0)
        return DiseasedStateEnum.None;
      if (diseaseLevel < 1)
        return DiseasedStateEnum.Normal;
      if (diseaseLevel < 3)
        return DiseasedStateEnum.Diseased;
      if (diseaseLevel < 4)
        return DiseasedStateEnum.Blocked;
      return diseaseLevel < 5 ? DiseasedStateEnum.Shelter : DiseasedStateEnum.None;
    }
  }
}

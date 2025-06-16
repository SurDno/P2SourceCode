namespace Engine.Common.DateTime
{
  public static class TimesOfDayUtility
  {
    public static bool HasValue(TimesOfDay source, TimesOfDay value) => (source & value) != 0;
  }
}

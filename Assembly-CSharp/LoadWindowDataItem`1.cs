using System;

[Serializable]
public class LoadWindowDataItem<T>
{
  public T Value;
  public int Weight;
  public bool LimitGameDay;
  public int MinGameDay;
  public int MaxGameDay;
  public bool LimitDeathCount;
  public int MinDeathCount;
  public int MaxDeathCount;
}

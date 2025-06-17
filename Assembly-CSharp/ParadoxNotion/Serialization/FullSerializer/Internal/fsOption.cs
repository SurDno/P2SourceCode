namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public static class fsOption
  {
    public static fsOption<T> Just<T>(T value) => new(value);
  }
}

namespace SteamNative
{
  internal struct HServerQuery
  {
    public int Value;

    public static implicit operator HServerQuery(int value)
    {
      return new HServerQuery { Value = value };
    }

    public static implicit operator int(HServerQuery value) => value.Value;
  }
}

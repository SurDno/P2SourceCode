namespace SteamNative
{
  internal struct SteamItemDef_t
  {
    public int Value;

    public static implicit operator SteamItemDef_t(int value)
    {
      return new SteamItemDef_t { Value = value };
    }

    public static implicit operator int(SteamItemDef_t value) => value.Value;
  }
}

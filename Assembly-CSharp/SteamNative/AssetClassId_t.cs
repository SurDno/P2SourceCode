namespace SteamNative
{
  internal struct AssetClassId_t
  {
    public ulong Value;

    public static implicit operator AssetClassId_t(ulong value)
    {
      return new AssetClassId_t { Value = value };
    }

    public static implicit operator ulong(AssetClassId_t value) => value.Value;
  }
}

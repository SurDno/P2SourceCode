namespace SteamNative
{
  internal struct BundleId_t
  {
    public uint Value;

    public static implicit operator BundleId_t(uint value)
    {
      return new BundleId_t { Value = value };
    }

    public static implicit operator uint(BundleId_t value) => value.Value;
  }
}

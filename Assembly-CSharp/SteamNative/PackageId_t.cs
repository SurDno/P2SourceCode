namespace SteamNative
{
  internal struct PackageId_t
  {
    public uint Value;

    public static implicit operator PackageId_t(uint value)
    {
      return new PackageId_t() { Value = value };
    }

    public static implicit operator uint(PackageId_t value) => value.Value;
  }
}

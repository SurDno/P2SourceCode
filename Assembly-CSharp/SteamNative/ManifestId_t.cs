namespace SteamNative
{
  internal struct ManifestId_t
  {
    public ulong Value;

    public static implicit operator ManifestId_t(ulong value)
    {
      return new ManifestId_t() { Value = value };
    }

    public static implicit operator ulong(ManifestId_t value) => value.Value;
  }
}

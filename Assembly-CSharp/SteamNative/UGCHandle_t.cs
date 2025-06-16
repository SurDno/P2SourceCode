namespace SteamNative
{
  internal struct UGCHandle_t
  {
    public ulong Value;

    public static implicit operator UGCHandle_t(ulong value)
    {
      return new UGCHandle_t { Value = value };
    }

    public static implicit operator ulong(UGCHandle_t value) => value.Value;
  }
}

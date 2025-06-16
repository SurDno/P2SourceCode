namespace SteamNative
{
  internal struct PublishedFileUpdateHandle_t
  {
    public ulong Value;

    public static implicit operator PublishedFileUpdateHandle_t(ulong value)
    {
      return new PublishedFileUpdateHandle_t {
        Value = value
      };
    }

    public static implicit operator ulong(PublishedFileUpdateHandle_t value) => value.Value;
  }
}

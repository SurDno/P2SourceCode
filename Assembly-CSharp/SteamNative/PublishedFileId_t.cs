namespace SteamNative
{
  internal struct PublishedFileId_t
  {
    public ulong Value;

    public static implicit operator PublishedFileId_t(ulong value)
    {
      return new PublishedFileId_t { Value = value };
    }

    public static implicit operator ulong(PublishedFileId_t value) => value.Value;
  }
}

namespace SteamNative
{
  internal struct AppId_t
  {
    public uint Value;

    public static implicit operator AppId_t(uint value)
    {
      return new AppId_t { Value = value };
    }

    public static implicit operator uint(AppId_t value) => value.Value;
  }
}

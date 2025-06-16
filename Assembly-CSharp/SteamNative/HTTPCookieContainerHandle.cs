namespace SteamNative
{
  internal struct HTTPCookieContainerHandle
  {
    public uint Value;

    public static implicit operator HTTPCookieContainerHandle(uint value)
    {
      return new HTTPCookieContainerHandle()
      {
        Value = value
      };
    }

    public static implicit operator uint(HTTPCookieContainerHandle value) => value.Value;
  }
}

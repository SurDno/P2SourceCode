namespace SteamNative
{
  internal struct HTTPRequestHandle
  {
    public uint Value;

    public static implicit operator HTTPRequestHandle(uint value)
    {
      return new HTTPRequestHandle { Value = value };
    }

    public static implicit operator uint(HTTPRequestHandle value) => value.Value;
  }
}

namespace SteamNative
{
  internal struct ScreenshotHandle
  {
    public uint Value;

    public static implicit operator ScreenshotHandle(uint value)
    {
      return new ScreenshotHandle { Value = value };
    }

    public static implicit operator uint(ScreenshotHandle value) => value.Value;
  }
}

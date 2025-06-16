namespace SteamNative
{
  internal struct HHTMLBrowser
  {
    public uint Value;

    public static implicit operator HHTMLBrowser(uint value)
    {
      return new HHTMLBrowser() { Value = value };
    }

    public static implicit operator uint(HHTMLBrowser value) => value.Value;
  }
}

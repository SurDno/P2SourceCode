namespace SteamNative
{
  internal struct HSteamPipe
  {
    public int Value;

    public static implicit operator HSteamPipe(int value)
    {
      return new HSteamPipe { Value = value };
    }

    public static implicit operator int(HSteamPipe value) => value.Value;
  }
}

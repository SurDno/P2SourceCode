namespace SteamNative
{
  internal struct RTime32
  {
    public uint Value;

    public static implicit operator RTime32(uint value)
    {
      return new RTime32() { Value = value };
    }

    public static implicit operator uint(RTime32 value) => value.Value;
  }
}

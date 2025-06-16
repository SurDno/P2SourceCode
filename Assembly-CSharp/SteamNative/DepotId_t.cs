namespace SteamNative
{
  internal struct DepotId_t
  {
    public uint Value;

    public static implicit operator DepotId_t(uint value)
    {
      return new DepotId_t() { Value = value };
    }

    public static implicit operator uint(DepotId_t value) => value.Value;
  }
}

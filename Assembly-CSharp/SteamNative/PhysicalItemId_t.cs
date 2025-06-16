namespace SteamNative
{
  internal struct PhysicalItemId_t
  {
    public uint Value;

    public static implicit operator PhysicalItemId_t(uint value)
    {
      return new PhysicalItemId_t() { Value = value };
    }

    public static implicit operator uint(PhysicalItemId_t value) => value.Value;
  }
}

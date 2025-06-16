namespace SteamNative
{
  internal struct HAuthTicket
  {
    public uint Value;

    public static implicit operator HAuthTicket(uint value)
    {
      return new HAuthTicket { Value = value };
    }

    public static implicit operator uint(HAuthTicket value) => value.Value;
  }
}

namespace SteamNative
{
  internal struct ControllerDigitalActionHandle_t
  {
    public ulong Value;

    public static implicit operator ControllerDigitalActionHandle_t(ulong value)
    {
      return new ControllerDigitalActionHandle_t()
      {
        Value = value
      };
    }

    public static implicit operator ulong(ControllerDigitalActionHandle_t value) => value.Value;
  }
}

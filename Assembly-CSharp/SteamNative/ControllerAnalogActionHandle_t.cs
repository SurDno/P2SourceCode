namespace SteamNative
{
  internal struct ControllerAnalogActionHandle_t
  {
    public ulong Value;

    public static implicit operator ControllerAnalogActionHandle_t(ulong value)
    {
      return new ControllerAnalogActionHandle_t {
        Value = value
      };
    }

    public static implicit operator ulong(ControllerAnalogActionHandle_t value) => value.Value;
  }
}

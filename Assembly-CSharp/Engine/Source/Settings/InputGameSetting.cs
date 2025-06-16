using Inspectors;

namespace Engine.Source.Settings
{
  public class InputGameSetting : SettingsInstanceByRequest<InputGameSetting>
  {
    [Inspected]
    public IValue<float> MouseSensitivity = new FloatValue(nameof (MouseSensitivity), 0.5f, 0.0f, 1f);
    [Inspected]
    public IValue<float> JoystickSensitivity = new FloatValue(nameof (JoystickSensitivity), 0.3f, 0.1f, 1f);
    [Inspected]
    public IValue<bool> MouseInvert = new BoolValue(nameof (MouseInvert));
    [Inspected]
    public IValue<bool> JoystickInvert = new BoolValue(nameof (JoystickInvert));
    [Inspected]
    public ListKeyItems KeySettings = new ListKeyItems(nameof (KeySettings));
    [Inspected]
    public IValue<int> JoystickLayout = new IntValue(nameof (JoystickLayout), minValue: 0, maxValue: 2);

    public void GetValuesFromStorage()
    {
      MouseSensitivity = new FloatValue("MouseSensitivity", 0.5f, 0.0f, 1f);
      JoystickSensitivity = new FloatValue("JoystickSensitivity", 0.3f, 0.1f, 1f);
      MouseInvert = new BoolValue("MouseInvert");
      JoystickInvert = new BoolValue("JoystickInvert");
      KeySettings = new ListKeyItems("KeySettings");
      JoystickLayout = new IntValue("JoystickLayout", minValue: 0, maxValue: 2);
      Apply();
    }
  }
}

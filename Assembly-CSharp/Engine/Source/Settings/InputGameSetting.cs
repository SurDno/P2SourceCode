using Inspectors;

namespace Engine.Source.Settings
{
  public class InputGameSetting : SettingsInstanceByRequest<InputGameSetting>
  {
    [Inspected]
    public IValue<float> MouseSensitivity = (IValue<float>) new FloatValue(nameof (MouseSensitivity), 0.5f, 0.0f, 1f);
    [Inspected]
    public IValue<float> JoystickSensitivity = (IValue<float>) new FloatValue(nameof (JoystickSensitivity), 0.3f, 0.1f, 1f);
    [Inspected]
    public IValue<bool> MouseInvert = (IValue<bool>) new BoolValue(nameof (MouseInvert));
    [Inspected]
    public IValue<bool> JoystickInvert = (IValue<bool>) new BoolValue(nameof (JoystickInvert));
    [Inspected]
    public ListKeyItems KeySettings = new ListKeyItems(nameof (KeySettings));
    [Inspected]
    public IValue<int> JoystickLayout = (IValue<int>) new IntValue(nameof (JoystickLayout), minValue: 0, maxValue: 2);

    public void GetValuesFromStorage()
    {
      this.MouseSensitivity = (IValue<float>) new FloatValue("MouseSensitivity", 0.5f, 0.0f, 1f);
      this.JoystickSensitivity = (IValue<float>) new FloatValue("JoystickSensitivity", 0.3f, 0.1f, 1f);
      this.MouseInvert = (IValue<bool>) new BoolValue("MouseInvert");
      this.JoystickInvert = (IValue<bool>) new BoolValue("JoystickInvert");
      this.KeySettings = new ListKeyItems("KeySettings");
      this.JoystickLayout = (IValue<int>) new IntValue("JoystickLayout", minValue: 0, maxValue: 2);
      this.Apply();
    }
  }
}

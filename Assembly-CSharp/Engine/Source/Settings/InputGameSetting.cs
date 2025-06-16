// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.InputGameSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;

#nullable disable
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

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Settings.External;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ExternalInputSettings))]
  public class ExternalInputSettings_Generated : 
    ExternalInputSettings,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ExternalInputSettings_Generated instance = Activator.CreateInstance<ExternalInputSettings_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ExternalInputSettings_Generated settingsGenerated = (ExternalInputSettings_Generated) target2;
      settingsGenerated.Version = this.Version;
      settingsGenerated.FlyWalkSpeed = this.FlyWalkSpeed;
      settingsGenerated.FlyRunSpeed = this.FlyRunSpeed;
      settingsGenerated.FlySensitivity = this.FlySensitivity;
      settingsGenerated.SmoothMove = this.SmoothMove;
      settingsGenerated.SmoothMoveTime = this.SmoothMoveTime;
      settingsGenerated.MinMouseSensitivity = this.MinMouseSensitivity;
      settingsGenerated.MaxMouseSensitivity = this.MaxMouseSensitivity;
      settingsGenerated.WheelMouseSensitivity = this.WheelMouseSensitivity;
      settingsGenerated.JoystickSensitivity = this.JoystickSensitivity;
      settingsGenerated.JoystickScaleSensitivity = this.JoystickScaleSensitivity;
      settingsGenerated.VirtualMouseSensitivity = this.VirtualMouseSensitivity;
      settingsGenerated.HoldDelay = this.HoldDelay;
      settingsGenerated.UseJoystick = this.UseJoystick;
      settingsGenerated.UseArrow = this.UseArrow;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", this.Version);
      DefaultDataWriteUtility.Write(writer, "FlyWalkSpeed", this.FlyWalkSpeed);
      DefaultDataWriteUtility.Write(writer, "FlyRunSpeed", this.FlyRunSpeed);
      DefaultDataWriteUtility.Write(writer, "FlySensitivity", this.FlySensitivity);
      DefaultDataWriteUtility.Write(writer, "SmoothMove", this.SmoothMove);
      DefaultDataWriteUtility.Write(writer, "SmoothMoveTime", this.SmoothMoveTime);
      DefaultDataWriteUtility.Write(writer, "MinMouseSensitivity", this.MinMouseSensitivity);
      DefaultDataWriteUtility.Write(writer, "MaxMouseSensitivity", this.MaxMouseSensitivity);
      DefaultDataWriteUtility.Write(writer, "WheelMouseSensitivity", this.WheelMouseSensitivity);
      DefaultDataWriteUtility.Write(writer, "JoystickSensitivity", this.JoystickSensitivity);
      DefaultDataWriteUtility.Write(writer, "JoystickScaleSensitivity", this.JoystickScaleSensitivity);
      DefaultDataWriteUtility.Write(writer, "VirtualMouseSensitivity", this.VirtualMouseSensitivity);
      DefaultDataWriteUtility.Write(writer, "HoldDelay", this.HoldDelay);
      DefaultDataWriteUtility.Write(writer, "UseJoystick", this.UseJoystick);
      DefaultDataWriteUtility.Write(writer, "UseArrow", this.UseArrow);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Version = DefaultDataReadUtility.Read(reader, "Version", this.Version);
      this.FlyWalkSpeed = DefaultDataReadUtility.Read(reader, "FlyWalkSpeed", this.FlyWalkSpeed);
      this.FlyRunSpeed = DefaultDataReadUtility.Read(reader, "FlyRunSpeed", this.FlyRunSpeed);
      this.FlySensitivity = DefaultDataReadUtility.Read(reader, "FlySensitivity", this.FlySensitivity);
      this.SmoothMove = DefaultDataReadUtility.Read(reader, "SmoothMove", this.SmoothMove);
      this.SmoothMoveTime = DefaultDataReadUtility.Read(reader, "SmoothMoveTime", this.SmoothMoveTime);
      this.MinMouseSensitivity = DefaultDataReadUtility.Read(reader, "MinMouseSensitivity", this.MinMouseSensitivity);
      this.MaxMouseSensitivity = DefaultDataReadUtility.Read(reader, "MaxMouseSensitivity", this.MaxMouseSensitivity);
      this.WheelMouseSensitivity = DefaultDataReadUtility.Read(reader, "WheelMouseSensitivity", this.WheelMouseSensitivity);
      this.JoystickSensitivity = DefaultDataReadUtility.Read(reader, "JoystickSensitivity", this.JoystickSensitivity);
      this.JoystickScaleSensitivity = DefaultDataReadUtility.Read(reader, "JoystickScaleSensitivity", this.JoystickScaleSensitivity);
      this.VirtualMouseSensitivity = DefaultDataReadUtility.Read(reader, "VirtualMouseSensitivity", this.VirtualMouseSensitivity);
      this.HoldDelay = DefaultDataReadUtility.Read(reader, "HoldDelay", this.HoldDelay);
      this.UseJoystick = DefaultDataReadUtility.Read(reader, "UseJoystick", this.UseJoystick);
      this.UseArrow = DefaultDataReadUtility.Read(reader, "UseArrow", this.UseArrow);
    }
  }
}

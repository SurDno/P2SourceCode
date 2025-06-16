using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Settings.External;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ExternalInputSettings_Generated settingsGenerated = (ExternalInputSettings_Generated) target2;
      settingsGenerated.Version = Version;
      settingsGenerated.FlyWalkSpeed = FlyWalkSpeed;
      settingsGenerated.FlyRunSpeed = FlyRunSpeed;
      settingsGenerated.FlySensitivity = FlySensitivity;
      settingsGenerated.SmoothMove = SmoothMove;
      settingsGenerated.SmoothMoveTime = SmoothMoveTime;
      settingsGenerated.MinMouseSensitivity = MinMouseSensitivity;
      settingsGenerated.MaxMouseSensitivity = MaxMouseSensitivity;
      settingsGenerated.WheelMouseSensitivity = WheelMouseSensitivity;
      settingsGenerated.JoystickSensitivity = JoystickSensitivity;
      settingsGenerated.JoystickScaleSensitivity = JoystickScaleSensitivity;
      settingsGenerated.VirtualMouseSensitivity = VirtualMouseSensitivity;
      settingsGenerated.HoldDelay = HoldDelay;
      settingsGenerated.UseJoystick = UseJoystick;
      settingsGenerated.UseArrow = UseArrow;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", Version);
      DefaultDataWriteUtility.Write(writer, "FlyWalkSpeed", FlyWalkSpeed);
      DefaultDataWriteUtility.Write(writer, "FlyRunSpeed", FlyRunSpeed);
      DefaultDataWriteUtility.Write(writer, "FlySensitivity", FlySensitivity);
      DefaultDataWriteUtility.Write(writer, "SmoothMove", SmoothMove);
      DefaultDataWriteUtility.Write(writer, "SmoothMoveTime", SmoothMoveTime);
      DefaultDataWriteUtility.Write(writer, "MinMouseSensitivity", MinMouseSensitivity);
      DefaultDataWriteUtility.Write(writer, "MaxMouseSensitivity", MaxMouseSensitivity);
      DefaultDataWriteUtility.Write(writer, "WheelMouseSensitivity", WheelMouseSensitivity);
      DefaultDataWriteUtility.Write(writer, "JoystickSensitivity", JoystickSensitivity);
      DefaultDataWriteUtility.Write(writer, "JoystickScaleSensitivity", JoystickScaleSensitivity);
      DefaultDataWriteUtility.Write(writer, "VirtualMouseSensitivity", VirtualMouseSensitivity);
      DefaultDataWriteUtility.Write(writer, "HoldDelay", HoldDelay);
      DefaultDataWriteUtility.Write(writer, "UseJoystick", UseJoystick);
      DefaultDataWriteUtility.Write(writer, "UseArrow", UseArrow);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Version = DefaultDataReadUtility.Read(reader, "Version", Version);
      FlyWalkSpeed = DefaultDataReadUtility.Read(reader, "FlyWalkSpeed", FlyWalkSpeed);
      FlyRunSpeed = DefaultDataReadUtility.Read(reader, "FlyRunSpeed", FlyRunSpeed);
      FlySensitivity = DefaultDataReadUtility.Read(reader, "FlySensitivity", FlySensitivity);
      SmoothMove = DefaultDataReadUtility.Read(reader, "SmoothMove", SmoothMove);
      SmoothMoveTime = DefaultDataReadUtility.Read(reader, "SmoothMoveTime", SmoothMoveTime);
      MinMouseSensitivity = DefaultDataReadUtility.Read(reader, "MinMouseSensitivity", MinMouseSensitivity);
      MaxMouseSensitivity = DefaultDataReadUtility.Read(reader, "MaxMouseSensitivity", MaxMouseSensitivity);
      WheelMouseSensitivity = DefaultDataReadUtility.Read(reader, "WheelMouseSensitivity", WheelMouseSensitivity);
      JoystickSensitivity = DefaultDataReadUtility.Read(reader, "JoystickSensitivity", JoystickSensitivity);
      JoystickScaleSensitivity = DefaultDataReadUtility.Read(reader, "JoystickScaleSensitivity", JoystickScaleSensitivity);
      VirtualMouseSensitivity = DefaultDataReadUtility.Read(reader, "VirtualMouseSensitivity", VirtualMouseSensitivity);
      HoldDelay = DefaultDataReadUtility.Read(reader, "HoldDelay", HoldDelay);
      UseJoystick = DefaultDataReadUtility.Read(reader, "UseJoystick", UseJoystick);
      UseArrow = DefaultDataReadUtility.Read(reader, "UseArrow", UseArrow);
    }
  }
}

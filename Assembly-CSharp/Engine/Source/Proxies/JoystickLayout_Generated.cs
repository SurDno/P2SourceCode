using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (JoystickLayout))]
  public class JoystickLayout_Generated : 
    JoystickLayout,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      JoystickLayout_Generated instance = Activator.CreateInstance<JoystickLayout_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      JoystickLayout_Generated joystickLayoutGenerated = (JoystickLayout_Generated) target2;
      joystickLayoutGenerated.Name = this.Name;
      CloneableObjectUtility.CopyListTo<AxisBind>(joystickLayoutGenerated.Axes, this.Axes);
      CloneableObjectUtility.CopyListTo<AxisToButton>(joystickLayoutGenerated.AxesToButtons, this.AxesToButtons);
      CloneableObjectUtility.CopyListTo<KeyToButton>(joystickLayoutGenerated.KeysToButtons, this.KeysToButtons);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteListSerialize<AxisBind>(writer, "Axes", this.Axes);
      DefaultDataWriteUtility.WriteListSerialize<AxisToButton>(writer, "AxesToButtons", this.AxesToButtons);
      DefaultDataWriteUtility.WriteListSerialize<KeyToButton>(writer, "KeysToButtons", this.KeysToButtons);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Axes = DefaultDataReadUtility.ReadListSerialize<AxisBind>(reader, "Axes", this.Axes);
      this.AxesToButtons = DefaultDataReadUtility.ReadListSerialize<AxisToButton>(reader, "AxesToButtons", this.AxesToButtons);
      this.KeysToButtons = DefaultDataReadUtility.ReadListSerialize<KeyToButton>(reader, "KeysToButtons", this.KeysToButtons);
    }
  }
}

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using InputServices;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (KeyToButton))]
  public class KeyToButton_Generated : 
    KeyToButton,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      KeyToButton_Generated instance = Activator.CreateInstance<KeyToButton_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      KeyToButton_Generated toButtonGenerated = (KeyToButton_Generated) target2;
      toButtonGenerated.Name = this.Name;
      toButtonGenerated.KeyCode = this.KeyCode;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteEnum<JoystickKeyCode>(writer, "KeyCode", this.KeyCode);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.KeyCode = DefaultDataReadUtility.ReadEnum<JoystickKeyCode>(reader, "KeyCode");
    }
  }
}

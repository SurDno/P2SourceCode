using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AxisToButton))]
  public class AxisToButton_Generated : 
    AxisToButton,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AxisToButton_Generated instance = Activator.CreateInstance<AxisToButton_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      AxisToButton_Generated toButtonGenerated = (AxisToButton_Generated) target2;
      toButtonGenerated.Name = this.Name;
      toButtonGenerated.Axis = this.Axis;
      toButtonGenerated.Min = this.Min;
      toButtonGenerated.Max = this.Max;
      toButtonGenerated.Inverse = this.Inverse;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.Write(writer, "Axis", this.Axis);
      DefaultDataWriteUtility.Write(writer, "Min", this.Min);
      DefaultDataWriteUtility.Write(writer, "Max", this.Max);
      DefaultDataWriteUtility.Write(writer, "Inverse", this.Inverse);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Axis = DefaultDataReadUtility.Read(reader, "Axis", this.Axis);
      this.Min = DefaultDataReadUtility.Read(reader, "Min", this.Min);
      this.Max = DefaultDataReadUtility.Read(reader, "Max", this.Max);
      this.Inverse = DefaultDataReadUtility.Read(reader, "Inverse", this.Inverse);
    }
  }
}

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AxisBind))]
  public class AxisBind_Generated : 
    AxisBind,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AxisBind_Generated instance = Activator.CreateInstance<AxisBind_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      AxisBind_Generated axisBindGenerated = (AxisBind_Generated) target2;
      axisBindGenerated.Name = this.Name;
      axisBindGenerated.Axis = this.Axis;
      axisBindGenerated.Dead = this.Dead;
      axisBindGenerated.Normalize = this.Normalize;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.Write(writer, "Axis", this.Axis);
      DefaultDataWriteUtility.Write(writer, "Dead", this.Dead);
      DefaultDataWriteUtility.Write(writer, "Normalize", this.Normalize);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Axis = DefaultDataReadUtility.Read(reader, "Axis", this.Axis);
      this.Dead = DefaultDataReadUtility.Read(reader, "Dead", this.Dead);
      this.Normalize = DefaultDataReadUtility.Read(reader, "Normalize", this.Normalize);
    }
  }
}

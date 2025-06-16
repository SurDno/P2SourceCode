using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      AxisBind_Generated axisBindGenerated = (AxisBind_Generated) target2;
      axisBindGenerated.Name = Name;
      axisBindGenerated.Axis = Axis;
      axisBindGenerated.Dead = Dead;
      axisBindGenerated.Normalize = Normalize;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", Name);
      DefaultDataWriteUtility.Write(writer, "Axis", Axis);
      DefaultDataWriteUtility.Write(writer, "Dead", Dead);
      DefaultDataWriteUtility.Write(writer, "Normalize", Normalize);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Name = DefaultDataReadUtility.Read(reader, "Name", Name);
      Axis = DefaultDataReadUtility.Read(reader, "Axis", Axis);
      Dead = DefaultDataReadUtility.Read(reader, "Dead", Dead);
      Normalize = DefaultDataReadUtility.Read(reader, "Normalize", Normalize);
    }
  }
}

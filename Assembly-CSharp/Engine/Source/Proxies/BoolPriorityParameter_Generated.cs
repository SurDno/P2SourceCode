using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BoolPriorityParameter))]
  public class BoolPriorityParameter_Generated : 
    BoolPriorityParameter,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      BoolPriorityParameter_Generated instance = Activator.CreateInstance<BoolPriorityParameter_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      BoolPriorityParameter_Generated parameterGenerated = (BoolPriorityParameter_Generated) target2;
      parameterGenerated.name = name;
      parameterGenerated.container = CloneableObjectUtility.Clone(container);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.WriteSerialize(writer, "Container", container);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      container = DefaultDataReadUtility.ReadSerialize<PriorityContainer<bool>>(reader, "Container");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultStateSaveUtility.SaveSerialize(writer, "Container", container);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      container = DefaultStateLoadUtility.ReadSerialize<PriorityContainer<bool>>(reader, "Container");
    }
  }
}

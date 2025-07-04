﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (FastTravelPointParameter))]
  public class FastTravelPointParameter_Generated : 
    FastTravelPointParameter,
    IComputeNeedSave,
    INeedSave,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public bool NeedSave { get; private set; } = true;

    public void ComputeNeedSave(object target2)
    {
      NeedSave = true;
      FastTravelPointParameter_Generated parameterGenerated = (FastTravelPointParameter_Generated) target2;
      if (parameterGenerated.name != name || parameterGenerated.value != value)
        return;
      NeedSave = false;
    }

    public object Clone()
    {
      FastTravelPointParameter_Generated instance = Activator.CreateInstance<FastTravelPointParameter_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      FastTravelPointParameter_Generated parameterGenerated = (FastTravelPointParameter_Generated) target2;
      parameterGenerated.name = name;
      parameterGenerated.value = value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.WriteEnum(writer, "Value", value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      value = DefaultDataReadUtility.ReadEnum<FastTravelPointEnum>(reader, "Value");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.WriteEnum(writer, "Value", value);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      value = DefaultDataReadUtility.ReadEnum<FastTravelPointEnum>(reader, "Value");
    }
  }
}

using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DynamicModelComponent))]
  public class DynamicModelComponent_Generated : 
    DynamicModelComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      DynamicModelComponent_Generated instance = Activator.CreateInstance<DynamicModelComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      DynamicModelComponent_Generated componentGenerated = (DynamicModelComponent_Generated) target2;
      componentGenerated.isEnabled = isEnabled;
      componentGenerated.model = model;
      CloneableObjectUtility.FillListTo(componentGenerated.models, models);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
      UnityDataWriteUtility.Write(writer, "Model", model);
      UnityDataWriteUtility.WriteList(writer, "Models", models);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
      model = UnityDataReadUtility.Read(reader, "Model", model);
      models = UnityDataReadUtility.ReadList(reader, "Models", models);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
      UnityDataWriteUtility.Write(writer, "Model", model);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
      model = UnityDataReadUtility.Read(reader, "Model", model);
    }
  }
}

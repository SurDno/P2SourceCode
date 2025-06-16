using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Connections;
using Scripts.Tools.Serializations.Converters;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      DynamicModelComponent_Generated componentGenerated = (DynamicModelComponent_Generated) target2;
      componentGenerated.isEnabled = this.isEnabled;
      componentGenerated.model = this.model;
      CloneableObjectUtility.FillListTo<Typed<IModel>>(componentGenerated.models, this.models);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      UnityDataWriteUtility.Write<IModel>(writer, "Model", this.model);
      UnityDataWriteUtility.WriteList<IModel>(writer, "Models", this.models);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.model = UnityDataReadUtility.Read<IModel>(reader, "Model", this.model);
      this.models = UnityDataReadUtility.ReadList<IModel>(reader, "Models", this.models);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      UnityDataWriteUtility.Write<IModel>(writer, "Model", this.model);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.model = UnityDataReadUtility.Read<IModel>(reader, "Model", this.model);
    }
  }
}

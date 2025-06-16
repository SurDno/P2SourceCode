using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ParametersComponent))]
  public class ParametersComponent_Generated : 
    ParametersComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      ParametersComponent_Generated instance = Activator.CreateInstance<ParametersComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<IParameter>(((ParametersComponent) target2).parameters, this.parameters);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<IParameter>(writer, "Parameters", this.parameters);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.parameters = DefaultDataReadUtility.ReadListSerialize<IParameter>(reader, "Parameters", this.parameters);
    }

    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveListParameters(writer, "Parameters", this.parameters);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.parameters = CustomStateLoadUtility.LoadListParameters(reader, "Parameters", this.parameters);
    }
  }
}

using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DiseaseComponent))]
  public class DiseaseComponent_Generated : 
    DiseaseComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      DiseaseComponent_Generated instance = Activator.CreateInstance<DiseaseComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
    }

    public void DataWrite(IDataWriter writer)
    {
    }

    public void DataRead(IDataReader reader, Type type)
    {
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "DiseaseValue", diseaseValue);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      diseaseValue = DefaultDataReadUtility.Read(reader, "DiseaseValue", diseaseValue);
    }
  }
}

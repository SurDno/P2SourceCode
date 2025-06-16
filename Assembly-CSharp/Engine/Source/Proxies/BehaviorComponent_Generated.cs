using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BehaviorComponent))]
  public class BehaviorComponent_Generated : 
    BehaviorComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      BehaviorComponent_Generated instance = Activator.CreateInstance<BehaviorComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((BehaviorComponent) target2).behaviorTreeResource = behaviorTreeResource;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "BehaviorTree", behaviorTreeResource);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      behaviorTreeResource = UnityDataReadUtility.Read(reader, "BehaviorTree", behaviorTreeResource);
    }

    public void StateSave(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "BehaviorTreeResource", behaviorTreeResource);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      behaviorTreeResource = UnityDataReadUtility.Read(reader, "BehaviorTreeResource", behaviorTreeResource);
    }
  }
}

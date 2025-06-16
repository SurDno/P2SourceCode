// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.BehaviorComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((BehaviorComponent) target2).behaviorTreeResource = this.behaviorTreeResource;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<ExternalBehaviorTree>(writer, "BehaviorTree", this.behaviorTreeResource);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.behaviorTreeResource = UnityDataReadUtility.Read<ExternalBehaviorTree>(reader, "BehaviorTree", this.behaviorTreeResource);
    }

    public void StateSave(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<ExternalBehaviorTree>(writer, "BehaviorTreeResource", this.behaviorTreeResource);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.behaviorTreeResource = UnityDataReadUtility.Read<ExternalBehaviorTree>(reader, "BehaviorTreeResource", this.behaviorTreeResource);
    }
  }
}

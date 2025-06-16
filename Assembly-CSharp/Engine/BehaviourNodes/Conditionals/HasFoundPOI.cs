// Decompiled with JetBrains decompiler
// Type: Engine.BehaviourNodes.Conditionals.HasFoundPOI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("does parent sequence have poi object?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (HasFoundPOI))]
  public class HasFoundPOI : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.CustomTaskReference)]
    [DataWriteProxy(MemberEnum.CustomTaskReference)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public POISequence ReferencedPOISequence;

    public override void OnStart()
    {
      if (this.ReferencedPOISequence != null)
        return;
      Debug.LogWarning((object) ("poi sequence not connected to has found poi node! " + (object) this.Owner.gameObject));
    }

    public override TaskStatus OnUpdate()
    {
      return this.ReferencedPOISequence != null ? ((UnityEngine.Object) this.ReferencedPOISequence.OutPOI != (UnityEngine.Object) null ? TaskStatus.Success : TaskStatus.Failure) : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteTaskReference<POISequence>(writer, "ReferencedPOISequence", this.ReferencedPOISequence);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.ReferencedPOISequence = BehaviorTreeDataReadUtility.ReadTaskReference<POISequence>(reader, "ReferencedPOISequence", this.ReferencedPOISequence);
    }
  }
}

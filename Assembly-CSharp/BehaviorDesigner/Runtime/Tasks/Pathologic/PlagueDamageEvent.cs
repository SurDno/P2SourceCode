// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.PlagueDamageEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Не работает!!! Send plague damage event to Info component.")]
  [TaskCategory("Pathologic/Player")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (PlagueDamageEvent))]
  public class PlagueDamageEvent : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform WhoWillBeDamaged;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Use null if you are the damage source.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform WhoDamages;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat Amount = (SharedFloat) 1f;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.WhoWillBeDamaged.Value == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("{0}: WhoWillBeDamaged is null", (object) this.gameObject.name);
        return TaskStatus.Failure;
      }
      if (EntityUtility.GetEntity((UnityEngine.Object) this.WhoDamages.Value == (UnityEngine.Object) null ? this.gameObject : this.WhoDamages.Value.gameObject) == null)
      {
        Debug.LogWarningFormat("{0}: doesn't match any entity", (object) this.gameObject.name);
        return TaskStatus.Failure;
      }
      if (EntityUtility.GetEntity(this.WhoWillBeDamaged.Value.gameObject) != null)
        return TaskStatus.Success;
      Debug.LogWarningFormat("{0}: doesn't match any entity", (object) this.gameObject.name);
      return TaskStatus.Failure;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "WhoWillBeDamaged", this.WhoWillBeDamaged);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "WhoDamages", this.WhoDamages);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Amount", this.Amount);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.WhoWillBeDamaged = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "WhoWillBeDamaged", this.WhoWillBeDamaged);
      this.WhoDamages = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "WhoDamages", this.WhoDamages);
      this.Amount = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Amount", this.Amount);
    }
  }
}

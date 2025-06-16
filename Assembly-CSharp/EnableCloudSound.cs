// Decompiled with JetBrains decompiler
// Type: EnableCloudSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
[TaskDescription("Enable PlagueCloud sound.")]
[TaskCategory("Pathologic/PlagueCloud")]
[TaskIcon("Pathologic_PlagueCloudIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (EnableCloudSound))]
public class EnableCloudSound : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public SharedBool Enabled;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public SharedFloat MaxDistance = (SharedFloat) 50f;
  private PivotCloud pivot;

  public override TaskStatus OnUpdate()
  {
    if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
    {
      this.pivot = this.gameObject.GetComponent<PivotCloud>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (PivotCloud).Name + " component"), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
    }
    this.pivot.EnableSound(this.Enabled.Value);
    this.pivot.SetSoundMaxDistance(this.MaxDistance.Value);
    return TaskStatus.Success;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Enabled", this.Enabled);
    BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "MaxDistance", this.MaxDistance);
  }

  public void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.Enabled = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Enabled", this.Enabled);
    this.MaxDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "MaxDistance", this.MaxDistance);
  }
}

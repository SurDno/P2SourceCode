// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.RunDialog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Engine.Source.Utility;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Tryes repeatedly to start dialog with NPC (Запускает любой интеракт блюпринт)")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (RunDialog))]
  public class RunDialog : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Should the task return if the dialog start trial fails")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool EndOnFailure;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Interact blueprint prefab")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject blueprint;
    private bool failed = false;

    public override void OnAwake() => base.OnAwake();

    public override void OnStart()
    {
      this.failed = false;
      GameObject blueprintPrefab = this.blueprint.Value;
      if ((UnityEngine.Object) blueprintPrefab == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat(this.gameObject.name + ": has no blueprint");
        this.failed = true;
      }
      IEntity entity = EntityUtility.GetEntity(this.gameObject);
      if (entity == null)
      {
        Debug.LogWarningFormat(this.gameObject.name + ": null target");
        this.failed = true;
      }
      if (entity.GetComponent<ISpeakingComponent>() == null)
      {
        Debug.LogWarningFormat(this.gameObject.name + ": has no speaking component");
        this.failed = true;
      }
      BlueprintServiceUtility.Start(blueprintPrefab, entity, (System.Action) null, entity.GetInfo());
    }

    public override TaskStatus OnUpdate()
    {
      if (this.failed)
        return TaskStatus.Failure;
      return !PlayerUtility.IsPlayerCanControlling ? (this.EndOnFailure.Value ? TaskStatus.Failure : TaskStatus.Running) : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "EndOnFailure", this.EndOnFailure);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "Blueprint", this.blueprint);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.EndOnFailure = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "EndOnFailure", this.EndOnFailure);
      this.blueprint = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "Blueprint", this.blueprint);
    }
  }
}

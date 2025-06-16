using System;
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

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Tryes repeatedly to start dialog with NPC (Запускает любой интеракт блюпринт)")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (RunDialog))]
  public class RunDialog : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Should the task return if the dialog start trial fails")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool EndOnFailure;
    [Tooltip("Interact blueprint prefab")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedGameObject blueprint;
    private bool failed;

    public override void OnAwake() => base.OnAwake();

    public override void OnStart()
    {
      failed = false;
      GameObject blueprintPrefab = blueprint.Value;
      if ((UnityEngine.Object) blueprintPrefab == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat(gameObject.name + ": has no blueprint");
        failed = true;
      }
      IEntity entity = EntityUtility.GetEntity(gameObject);
      if (entity == null)
      {
        Debug.LogWarningFormat(gameObject.name + ": null target");
        failed = true;
      }
      if (entity.GetComponent<ISpeakingComponent>() == null)
      {
        Debug.LogWarningFormat(gameObject.name + ": has no speaking component");
        failed = true;
      }
      BlueprintServiceUtility.Start(blueprintPrefab, entity, null, entity.GetInfo());
    }

    public override TaskStatus OnUpdate()
    {
      if (failed)
        return TaskStatus.Failure;
      return !PlayerUtility.IsPlayerCanControlling ? (EndOnFailure.Value ? TaskStatus.Failure : TaskStatus.Running) : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "EndOnFailure", EndOnFailure);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Blueprint", blueprint);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      EndOnFailure = BehaviorTreeDataReadUtility.ReadShared(reader, "EndOnFailure", EndOnFailure);
      blueprint = BehaviorTreeDataReadUtility.ReadShared(reader, "Blueprint", blueprint);
    }
  }
}

using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [FactoryProxy(typeof (StopBehaviorTree))]
  [TaskDescription("Pause or disable a behavior tree and return success after it has been stopped.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=21")]
  [TaskIcon("{SkinColor}StopBehaviorTreeIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class StopBehaviorTree : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The GameObject of the behavior tree that should be stopped. If null use the current behavior")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedGameObject behaviorGameObject;
    [Tooltip("The group of the behavior tree that should be stopped")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt group;
    [Tooltip("Should the behavior be paused or completely disabled")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool pauseBehavior = false;
    private BehaviorTree behavior;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "BehaviorGameObject", behaviorGameObject);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Group", group);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PauseBehavior", pauseBehavior);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      behaviorGameObject = BehaviorTreeDataReadUtility.ReadShared(reader, "BehaviorGameObject", behaviorGameObject);
      group = BehaviorTreeDataReadUtility.ReadShared(reader, "Group", group);
      pauseBehavior = BehaviorTreeDataReadUtility.ReadShared(reader, "PauseBehavior", pauseBehavior);
    }

    public override void OnStart()
    {
      BehaviorTree[] components = GetDefaultGameObject(behaviorGameObject.Value).GetComponents<BehaviorTree>();
      if (components.Length == 1)
      {
        behavior = components[0];
      }
      else
      {
        if (components.Length <= 1 || !((UnityEngine.Object) behavior == (UnityEngine.Object) null))
          return;
        behavior = components[0];
      }
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) behavior == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      behavior.DisableBehavior(pauseBehavior.Value);
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      behaviorGameObject = null;
      group = 0;
      pauseBehavior = false;
    }
  }
}

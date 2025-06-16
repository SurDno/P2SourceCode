using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Look at target ON")]
  [TaskCategory("Pathologic/IK")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (LookAtOn))]
  public class LookAtOn : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool EyeContactOnly = false;
    [Tooltip("Stops looking at if target is out of limits")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool StopIfOutOfLimits = true;
    private IKController ikController;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) ikController == (UnityEngine.Object) null)
      {
        ikController = gameObject.GetComponent<IKController>();
        if ((UnityEngine.Object) ikController == (UnityEngine.Object) null)
        {
          Debug.LogError((object) (gameObject.name + ": doesn't contain " + typeof (IKController).Name + " unity component"), (UnityEngine.Object) gameObject);
          return TaskStatus.Failure;
        }
      }
      if ((UnityEngine.Object) Target.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      ikController.enabled = true;
      ikController.LookTarget = Target.Value;
      ikController.LookEyeContactOnly = EyeContactOnly.Value;
      ikController.StopIfOutOfLimits = StopIfOutOfLimits.Value;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "EyeContactOnly", EyeContactOnly);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "StopIfOutOfLimits", StopIfOutOfLimits);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      EyeContactOnly = BehaviorTreeDataReadUtility.ReadShared(reader, "EyeContactOnly", EyeContactOnly);
      StopIfOutOfLimits = BehaviorTreeDataReadUtility.ReadShared(reader, "StopIfOutOfLimits", StopIfOutOfLimits);
    }
  }
}

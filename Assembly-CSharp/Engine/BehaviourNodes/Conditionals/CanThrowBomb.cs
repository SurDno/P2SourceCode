using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("can throw bomb?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (CanThrowBomb))]
  public class CanThrowBomb : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Target;
    private NPCEnemy npcEnemy;

    public override void OnAwake() => npcEnemy = this.GetComponent<NPCEnemy>();

    public override TaskStatus OnUpdate()
    {
      return (UnityEngine.Object) npcEnemy == (UnityEngine.Object) null || (UnityEngine.Object) Target.Value == (UnityEngine.Object) null ? TaskStatus.Failure : (CanThrow() ? TaskStatus.Success : TaskStatus.Failure);
    }

    private bool CanThrow()
    {
      float v = 15f;
      float h = 1.5f;
      Vector3 vector3 = Target.Value.position - npcEnemy.transform.position;
      float angle1;
      float angle2;
      if (BomberHelper.CalcThrowAngles(v, vector3.magnitude, h, out angle1, out angle2))
      {
        Vector3 startPosition = npcEnemy.transform.position + Vector3.up * h;
        Vector3 normalized = vector3.normalized;
        float num = float.MinValue;
        if (BomberHelper.SphereCastParabola(angle1, v, h, startPosition, normalized))
          num = angle1;
        else if (BomberHelper.SphereCastParabola(angle2, v, h, startPosition, normalized))
          num = angle2;
        if (num != -3.4028234663852886E+38)
          return true;
      }
      return false;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
    }
  }
}

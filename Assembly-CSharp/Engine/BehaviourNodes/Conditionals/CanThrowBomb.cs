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

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("can throw bomb?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (CanThrowBomb))]
  public class CanThrowBomb : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    private NPCEnemy npcEnemy;

    public override void OnAwake() => this.npcEnemy = this.GetComponent<NPCEnemy>();

    public override TaskStatus OnUpdate()
    {
      return (UnityEngine.Object) this.npcEnemy == (UnityEngine.Object) null || (UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null ? TaskStatus.Failure : (this.CanThrow() ? TaskStatus.Success : TaskStatus.Failure);
    }

    private bool CanThrow()
    {
      float v = 15f;
      float h = 1.5f;
      Vector3 vector3 = this.Target.Value.position - this.npcEnemy.transform.position;
      float angle1;
      float angle2;
      if (BomberHelper.CalcThrowAngles(v, vector3.magnitude, h, out angle1, out angle2))
      {
        Vector3 startPosition = this.npcEnemy.transform.position + Vector3.up * h;
        Vector3 normalized = vector3.normalized;
        float num = float.MinValue;
        if (BomberHelper.SphereCastParabola(angle1, v, h, startPosition, normalized))
          num = angle1;
        else if (BomberHelper.SphereCastParabola(angle2, v, h, startPosition, normalized))
          num = angle2;
        if ((double) num != -3.4028234663852886E+38)
          return true;
      }
      return false;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
    }
  }
}

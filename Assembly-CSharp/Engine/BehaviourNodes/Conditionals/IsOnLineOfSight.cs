// Decompiled with JetBrains decompiler
// Type: Engine.BehaviourNodes.Conditionals.IsOnLineOfSight
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
namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Can raycast to NPC?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsOnLineOfSight))]
  public class IsOnLineOfSight : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public float RaycastDistance = 30f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public LayerMask physicsLayer = (LayerMask) (LayerMaskUtility.GetMask((LayerMask) 0) | LayerMaskUtility.GetMask((LayerMask) 9) | LayerMaskUtility.GetMask((LayerMask) 14));
    private Pivot pivot;

    public override void OnAwake()
    {
      this.pivot = this.gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} doesnt' contain {1} unity component", (object) this.gameObject.name, (object) typeof (Pivot).Name);
      }
      else
      {
        if (!((UnityEngine.Object) this.pivot.Head == (UnityEngine.Object) null))
          return;
        Debug.LogWarningFormat("{0} Pivot doesnt' contain Head GameObject", (object) this.gameObject.name);
      }
    }

    public override void OnStart()
    {
      if (!((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null))
        return;
      Debug.LogWarningFormat("{0} Target is null", (object) this.gameObject.name);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null || (UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null || (double) (this.Target.Value.transform.position - this.gameObject.transform.position).magnitude > (double) this.RaycastDistance)
        return TaskStatus.Failure;
      Vector3 origin = !((UnityEngine.Object) this.pivot.Head != (UnityEngine.Object) null) ? this.gameObject.transform.position : this.pivot.Head.transform.position;
      Pivot component = this.gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} doesnt' contain Pivot unity component");
        return TaskStatus.Failure;
      }
      Vector3 vector3 = !((UnityEngine.Object) component.Head != (UnityEngine.Object) null) ? this.gameObject.transform.position : component.Head.transform.position;
      return Physics.Raycast(origin, vector3 - origin, this.RaycastDistance, (int) this.physicsLayer, QueryTriggerInteraction.Ignore) ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      DefaultDataWriteUtility.Write(writer, "RaycastDistance", this.RaycastDistance);
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "PhysicsLayer", this.physicsLayer);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.RaycastDistance = DefaultDataReadUtility.Read(reader, "RaycastDistance", this.RaycastDistance);
      this.physicsLayer = BehaviorTreeDataReadUtility.ReadUnity(reader, "PhysicsLayer", this.physicsLayer);
    }
  }
}

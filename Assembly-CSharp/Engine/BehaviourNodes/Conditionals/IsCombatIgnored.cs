using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System.Reflection;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is NPC burning? (use null Target to check self)")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsCombatIgnored))]
  public class IsCombatIgnored : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    private GameObject cacheGameObject;
    private IParameter<bool> cacheParameter;

    public override TaskStatus OnUpdate()
    {
      GameObject gameObject = (UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null ? this.gameObject : this.Target.Value.gameObject;
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) this.cacheGameObject)
      {
        this.cacheGameObject = gameObject;
        this.cacheParameter = (IParameter<bool>) null;
        IEntity entity = !((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null) ? EntityUtility.GetEntity(this.Target.Value.gameObject) : EntityUtility.GetEntity(this.gameObject);
        if (entity == null)
        {
          Debug.LogWarning((object) (this.gameObject.name + " : entity not found, method : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
        ParametersComponent component = entity.GetComponent<ParametersComponent>();
        if (component == null)
          return TaskStatus.Failure;
        this.cacheParameter = component.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
      }
      return this.cacheParameter == null ? TaskStatus.Failure : (this.cacheParameter.Value ? TaskStatus.Success : TaskStatus.Failure);
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

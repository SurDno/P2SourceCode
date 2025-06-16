using System;
using System.Reflection;
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

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is NPC burning? (use null Target to check self)")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsCombatIgnored))]
  public class IsCombatIgnored : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Target;
    private GameObject cacheGameObject;
    private IParameter<bool> cacheParameter;

    public override TaskStatus OnUpdate()
    {
      GameObject gameObject = (UnityEngine.Object) Target.Value == (UnityEngine.Object) null ? this.gameObject : Target.Value.gameObject;
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) cacheGameObject)
      {
        cacheGameObject = gameObject;
        cacheParameter = null;
        IEntity entity = !((UnityEngine.Object) Target.Value == (UnityEngine.Object) null) ? EntityUtility.GetEntity(Target.Value.gameObject) : EntityUtility.GetEntity(this.gameObject);
        if (entity == null)
        {
          Debug.LogWarning((object) (this.gameObject.name + " : entity not found, method : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
        ParametersComponent component = entity.GetComponent<ParametersComponent>();
        if (component == null)
          return TaskStatus.Failure;
        cacheParameter = component.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
      }
      return cacheParameter == null ? TaskStatus.Failure : (cacheParameter.Value ? TaskStatus.Success : TaskStatus.Failure);
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

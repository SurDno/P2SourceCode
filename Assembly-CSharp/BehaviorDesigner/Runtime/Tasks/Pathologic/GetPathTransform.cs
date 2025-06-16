using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Get Spawnpoint")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetPathTransform))]
  public class GetPathTransform : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Spawnpoint;

    public override void OnStart()
    {
      IEntity owner = Owner.GetComponent<EngineGameObject>().Owner;
      if (owner == null)
      {
        Debug.LogWarningFormat("{0} has no entity", (object) gameObject.name);
      }
      else
      {
        NavigationComponent component1 = owner.GetComponent<NavigationComponent>();
        if (component1 != null)
        {
          IEntity setupPoint = component1.SetupPoint;
          if (setupPoint != null && (UnityEngine.Object) ((IEntityView) setupPoint).GameObject != (UnityEngine.Object) null)
          {
            PatrolPath component2 = ((IEntityView) setupPoint).GameObject.GetComponent<PatrolPath>();
            PathPart component3 = ((IEntityView) setupPoint).GameObject.GetComponent<PathPart>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null || (UnityEngine.Object) component3 != (UnityEngine.Object) null)
            {
              Spawnpoint.Value = ((IEntityView) setupPoint).GameObject.transform;
              return;
            }
          }
        }
        CrowdItemComponent component4 = owner.GetComponent<CrowdItemComponent>();
        if (component4 != null && component4.Point != null)
        {
          IEntity entityPoint = component4.Point.EntityPoint;
          if (entityPoint == null)
            return;
          GameObject gameObject = ((IEntityView) entityPoint).GameObject;
          if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
            return;
          PatrolPath component5 = gameObject.GetComponent<PatrolPath>();
          PathPart component6 = gameObject.GetComponent<PathPart>();
          if (!((UnityEngine.Object) component5 != (UnityEngine.Object) null) && !((UnityEngine.Object) component6 != (UnityEngine.Object) null))
            return;
          Spawnpoint.Value = gameObject.transform;
        }
        else
          Debug.LogError((object) (this.gameObject.name + " has no patrol path or path part!"));
      }
    }

    public override TaskStatus OnUpdate()
    {
      return !(bool) (UnityEngine.Object) Spawnpoint.Value ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Spawnpoint", Spawnpoint);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Spawnpoint = BehaviorTreeDataReadUtility.ReadShared(reader, "Spawnpoint", Spawnpoint);
    }
  }
}

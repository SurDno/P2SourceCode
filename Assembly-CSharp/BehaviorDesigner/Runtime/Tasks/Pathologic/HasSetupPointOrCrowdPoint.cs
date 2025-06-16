// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.HasSetupPointOrCrowdPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Has Setup point or crowd point GO")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (HasSetupPointOrCrowdPoint))]
  public class HasSetupPointOrCrowdPoint : 
    Conditional,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    private bool success = false;

    public override void OnStart()
    {
      IEntity owner = this.Owner.GetComponent<EngineGameObject>().Owner;
      if (owner == null)
        Debug.LogWarningFormat("{0} has no entity", (object) this.gameObject.name);
      else
        this.success = this.HasSetupPoint(owner) || this.HasCrowdPoint(owner);
    }

    private bool HasSetupPoint(IEntity entity)
    {
      NavigationComponent component = entity.GetComponent<NavigationComponent>();
      if (component == null)
        return false;
      IEntity setupPoint = component.SetupPoint;
      return setupPoint != null && (UnityEngine.Object) ((IEntityView) setupPoint).GameObject != (UnityEngine.Object) null;
    }

    private bool HasCrowdPoint(IEntity entity)
    {
      CrowdItemComponent component = entity.GetComponent<CrowdItemComponent>();
      if (component == null || component.Point == null)
        return false;
      IEntity entityPoint = component.Point.EntityPoint;
      return entityPoint != null && (UnityEngine.Object) ((IEntityView) entityPoint).GameObject != (UnityEngine.Object) null;
    }

    public override TaskStatus OnUpdate() => this.success ? TaskStatus.Success : TaskStatus.Failure;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    }
  }
}

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("POI Idle")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_IdleIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (POILoot))]
  public class POILoot : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat InPOITime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private List<string> lootContainers;
    private NpcState npcState;

    public override void OnAwake()
    {
      this.npcState = this.gameObject.GetComponent<NpcState>();
      if (!((UnityEngine.Object) this.npcState == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
    }

    public override void OnStart()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        return;
      this.npcState.Loot(this.InPOITime.Value, this.Target.Value.gameObject);
      this.TakeItems();
    }

    public override TaskStatus OnUpdate()
    {
      if (this.npcState.CurrentNpcState != NpcStateEnum.PointOfInterest)
        return TaskStatus.Failure;
      switch (this.npcState.Status)
      {
        case NpcStateStatusEnum.Success:
          return TaskStatus.Success;
        case NpcStateStatusEnum.Failed:
          return TaskStatus.Failure;
        default:
          return TaskStatus.Running;
      }
    }

    public override void OnEnd()
    {
    }

    private void TakeItems()
    {
      StorageComponent component1 = this.Owner?.GetComponent<EngineGameObject>()?.Owner?.GetComponent<StorageComponent>();
      StorageComponent component2 = this.Target.Value?.GetComponent<EngineGameObject>()?.Owner?.GetComponent<StorageComponent>();
      if (component1 == null || component2 == null)
        return;
      List<IStorableComponent> all1 = new List<IStorableComponent>(component2.Items).FindAll((Predicate<IStorableComponent>) (x => this.lootContainers.Exists((Predicate<string>) (y => y == x.Container.Owner.Template.Name))));
      List<IInventoryComponent> all2 = new List<IInventoryComponent>(component1.Containers).FindAll((Predicate<IInventoryComponent>) (x => this.lootContainers.Exists((Predicate<string>) (y => y == x.Owner.Template.Name))));
      foreach (IStorableComponent storableComponent in all1)
      {
        StorableComponent storable = (StorableComponent) storableComponent;
        if (storable != null && storable.Owner != null && storable.Storage != null)
        {
          foreach (IInventoryComponent container in all2)
          {
            if (component1 != null && container != null)
            {
              Intersect intersect = StorageUtility.GetIntersect((IStorageComponent) component1, container, storable, (Cell) null);
              if (intersect.IsAllowed)
              {
                ((StorageComponent) storable.Storage).MoveItem(storableComponent, intersect.Storage, intersect.Container, intersect.Cell.To());
                break;
              }
            }
          }
        }
      }
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "InPOITime", this.InPOITime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      DefaultDataWriteUtility.WriteList(writer, "LootContainers", this.lootContainers);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.InPOITime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "InPOITime", this.InPOITime);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.lootContainers = DefaultDataReadUtility.ReadList(reader, "LootContainers", this.lootContainers);
    }
  }
}

using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Inventory;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Engine.Source.Services
{
  [SaveDepend(typeof (ISimulation))]
  [GameService(new System.Type[] {typeof (DropBagService), typeof (IDropBagService)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class DropBagService : IDropBagService, ISavesController
  {
    [StateSaveProxy(MemberEnum.CustomListReference)]
    [StateLoadProxy(MemberEnum.CustomListReference)]
    [Inspected]
    protected List<IEntity> bags = new List<IEntity>();
    [Inspected]
    private List<IStorableComponent> waitings = new List<IStorableComponent>();

    public event Action<IEntity> OnCreateEntity;

    public event Action<IEntity> OnDeleteEntity;

    public void AddEntity(IEntity entity)
    {
      if (this.waitings.Count == 0)
        throw new Exception("No waiting entity");
      this.bags.Add(entity);
      List<IStorableComponent> list = this.waitings.ToList<IStorableComponent>();
      this.waitings.Clear();
      foreach (IStorableComponent storable in list)
        this.AddOrDrop(storable, entity);
      if (list.Count == this.waitings.Count)
      {
        this.waitings.Clear();
        throw new Exception("Drop cycle");
      }
    }

    private void AddOrDrop(IStorableComponent storable, IEntity entity)
    {
      if (storable.IsDisposed)
        return;
      Intersect intersect = StorageUtility.GetIntersect(entity.GetComponent<IStorageComponent>(), (IInventoryComponent) null, (StorableComponent) storable, (Cell) null);
      if (!intersect.IsAllowed)
      {
        Debug.Log((object) ObjectInfoUtility.GetStream().Append("Redrop storable : ").GetInfo((object) storable.Owner));
        this.DropBag(storable, entity);
      }
      else
      {
        Debug.Log((object) ObjectInfoUtility.GetStream().Append("Receve bag for storable : ").GetInfo((object) storable.Owner));
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        this.AddItem(storable, entity, intersect, player, true);
      }
    }

    public void Reset()
    {
      foreach (IEntity bag in this.bags.ToList<IEntity>())
      {
        if (bag == null)
          Debug.LogError((object) "Bag is null");
        else
          this.RemoveBag(bag);
      }
    }

    private void RemoveBag(IEntity bag)
    {
      if (this.bags.Contains(bag))
        this.bags.Remove(bag);
      Action<IEntity> onDeleteEntity = this.OnDeleteEntity;
      if (onDeleteEntity == null)
        return;
      onDeleteEntity(bag);
    }

    private IEnumerable<IEntity> FindExistingBags(IEntity target)
    {
      float searchRange = ExternalSettingsInstance<ExternalCommonSettings>.Instance.DropBagSearchRadius;
      foreach (IEntity bag1 in this.bags)
      {
        IEntity bag = bag1;
        if (bag == null)
          Debug.LogError((object) "Bug is null");
        else if (!bag.IsDisposed)
        {
          if ((double) (((IEntityView) bag).Position - ((IEntityView) target).Position).magnitude < (double) searchRange)
            yield return bag;
          bag = (IEntity) null;
        }
      }
    }

    public void TryDestroyDropBag(IStorageComponent storage)
    {
      if (storage == null || storage.IsDisposed)
        return;
      IEntity entity = storage.Owner;
      for (int index = 0; index < this.bags.Count; ++index)
      {
        if (entity == this.bags[index])
        {
          this.bags.RemoveAt(index);
          CoroutineService.Instance.WaitFrame((Action) (() => this.RemoveBag(entity)));
          break;
        }
      }
    }

    public void DropBag(IStorableComponent storable, IEntity owner)
    {
      if (storable.IsDisposed)
      {
        Debug.LogError((object) "Item is disposed");
      }
      else
      {
        IEntity entity = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DropBag.Value;
        if (entity == null)
          Debug.LogError((object) "Bag template not found");
        else if (ServiceLocator.GetService<ISimulation>().Player != owner)
        {
          storable.Owner.Dispose();
          Debug.LogWarning((object) ("Drop only player, owner : " + owner.GetInfo()));
        }
        else
        {
          foreach (IEntity existingBag in this.FindExistingBags(owner))
          {
            if (!existingBag.IsDisposed)
            {
              Intersect intersect = StorageUtility.GetIntersect(existingBag.GetComponent<IStorageComponent>(), (IInventoryComponent) null, (StorableComponent) storable, (Cell) null);
              if (intersect.IsAllowed)
              {
                this.AddItem(storable, existingBag, intersect, owner, false);
                return;
              }
            }
          }
          this.waitings.Add(storable);
          if (this.waitings.Count != 1)
            return;
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("Requre bag for storable : ").GetInfo((object) storable.Owner));
          DynamicModelComponent.GroupContext = "[Bugs]";
          Action<IEntity> onCreateEntity = this.OnCreateEntity;
          if (onCreateEntity != null)
            onCreateEntity(entity);
        }
      }
    }

    private void AddItem(
      IStorableComponent storable,
      IEntity bag,
      Intersect intersect,
      IEntity owner,
      bool teleport)
    {
      if (storable.IsDisposed)
        return;
      ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.ItemDrop, new object[1]
      {
        (object) storable.Owner.Template
      });
      bag.GetComponent<ContextComponent>()?.AddContext(owner.GetInfo());
      if (teleport)
      {
        NavigationComponent component1 = bag.GetComponent<NavigationComponent>();
        if (component1 != null)
        {
          LocationItemComponent component2 = owner.GetComponent<LocationItemComponent>();
          if (component2 != null)
          {
            float dropBagOffset = ExternalSettingsInstance<ExternalCommonSettings>.Instance.DropBagOffset;
            Vector3 vector3 = new Vector3(UnityEngine.Random.Range(-dropBagOffset, dropBagOffset), 0.0f, UnityEngine.Random.Range(-dropBagOffset, dropBagOffset));
            component1.TeleportTo(component2.Location, ((IEntityView) owner).Position + vector3, ((IEntityView) owner).Rotation);
          }
          else
            Debug.LogError((object) ("LocationItemComponent not found in " + bag.GetInfo()));
        }
        else
          Debug.LogError((object) ("INavigationComponent not found in " + bag.GetInfo()));
      }
      if (!(storable.Container == null ? intersect.Storage.AddItem((IStorableComponent) intersect.Storable, intersect.Container) : ((StorageComponent) storable.Storage).MoveItem(storable, intersect.Storage, intersect.Container, intersect.Cell.To())))
        throw new Exception();
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(this.GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(this.GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader((XmlNode) node, context);
        ((ISerializeStateLoad) this).StateLoad((IDataReader) reader, this.GetType());
        yield break;
      }
    }

    public void Unload() => this.bags.Clear();

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<DropBagService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.Source.Services;

[SaveDepend(typeof(ISimulation))]
[GameService(typeof(DropBagService), typeof(IDropBagService))]
[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class DropBagService : IDropBagService, ISavesController {
	[StateSaveProxy(MemberEnum.CustomListReference)] [StateLoadProxy(MemberEnum.CustomListReference)] [Inspected]
	protected List<IEntity> bags = new();

	[Inspected] private List<IStorableComponent> waitings = new();

	public event Action<IEntity> OnCreateEntity;

	public event Action<IEntity> OnDeleteEntity;

	public void AddEntity(IEntity entity) {
		if (waitings.Count == 0)
			throw new Exception("No waiting entity");
		bags.Add(entity);
		var list = waitings.ToList();
		waitings.Clear();
		foreach (var storable in list)
			AddOrDrop(storable, entity);
		if (list.Count == waitings.Count) {
			waitings.Clear();
			throw new Exception("Drop cycle");
		}
	}

	private void AddOrDrop(IStorableComponent storable, IEntity entity) {
		if (storable.IsDisposed)
			return;
		var intersect = StorageUtility.GetIntersect(entity.GetComponent<IStorageComponent>(), null,
			(StorableComponent)storable, null);
		if (!intersect.IsAllowed) {
			Debug.Log(ObjectInfoUtility.GetStream().Append("Redrop storable : ").GetInfo(storable.Owner));
			DropBag(storable, entity);
		} else {
			Debug.Log(ObjectInfoUtility.GetStream().Append("Receve bag for storable : ").GetInfo(storable.Owner));
			var player = ServiceLocator.GetService<ISimulation>().Player;
			AddItem(storable, entity, intersect, player, true);
		}
	}

	public void Reset() {
		foreach (var bag in bags.ToList())
			if (bag == null)
				Debug.LogError("Bag is null");
			else
				RemoveBag(bag);
	}

	private void RemoveBag(IEntity bag) {
		if (bags.Contains(bag))
			bags.Remove(bag);
		var onDeleteEntity = OnDeleteEntity;
		if (onDeleteEntity == null)
			return;
		onDeleteEntity(bag);
	}

	private IEnumerable<IEntity> FindExistingBags(IEntity target) {
		var searchRange = ExternalSettingsInstance<ExternalCommonSettings>.Instance.DropBagSearchRadius;
		foreach (var bag1 in bags) {
			var bag = bag1;
			if (bag == null)
				Debug.LogError("Bug is null");
			else if (!bag.IsDisposed) {
				if ((((IEntityView)bag).Position - ((IEntityView)target).Position).magnitude < (double)searchRange)
					yield return bag;
				bag = null;
			}
		}
	}

	public void TryDestroyDropBag(IStorageComponent storage) {
		if (storage == null || storage.IsDisposed)
			return;
		var entity = storage.Owner;
		for (var index = 0; index < bags.Count; ++index)
			if (entity == bags[index]) {
				bags.RemoveAt(index);
				CoroutineService.Instance.WaitFrame((Action)(() => RemoveBag(entity)));
				break;
			}
	}

	public void DropBag(IStorableComponent storable, IEntity owner) {
		if (storable.IsDisposed)
			Debug.LogError("Item is disposed");
		else {
			var entity = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DropBag.Value;
			if (entity == null)
				Debug.LogError("Bag template not found");
			else if (ServiceLocator.GetService<ISimulation>().Player != owner) {
				storable.Owner.Dispose();
				Debug.LogWarning("Drop only player, owner : " + owner.GetInfo());
			} else {
				foreach (var existingBag in FindExistingBags(owner))
					if (!existingBag.IsDisposed) {
						var intersect = StorageUtility.GetIntersect(existingBag.GetComponent<IStorageComponent>(), null,
							(StorableComponent)storable, null);
						if (intersect.IsAllowed) {
							AddItem(storable, existingBag, intersect, owner, false);
							return;
						}
					}

				waitings.Add(storable);
				if (waitings.Count != 1)
					return;
				Debug.Log(ObjectInfoUtility.GetStream().Append("Requre bag for storable : ").GetInfo(storable.Owner));
				DynamicModelComponent.GroupContext = "[Bugs]";
				var onCreateEntity = OnCreateEntity;
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
		bool teleport) {
		if (storable.IsDisposed)
			return;
		ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.ItemDrop, storable.Owner.Template);
		bag.GetComponent<ContextComponent>()?.AddContext(owner.GetInfo());
		if (teleport) {
			var component1 = bag.GetComponent<NavigationComponent>();
			if (component1 != null) {
				var component2 = owner.GetComponent<LocationItemComponent>();
				if (component2 != null) {
					var dropBagOffset = ExternalSettingsInstance<ExternalCommonSettings>.Instance.DropBagOffset;
					var vector3 = new Vector3(Random.Range(-dropBagOffset, dropBagOffset), 0.0f,
						Random.Range(-dropBagOffset, dropBagOffset));
					component1.TeleportTo(component2.Location, ((IEntityView)owner).Position + vector3,
						((IEntityView)owner).Rotation);
				} else
					Debug.LogError("LocationItemComponent not found in " + bag.GetInfo());
			} else
				Debug.LogError("INavigationComponent not found in " + bag.GetInfo());
		}

		if (!(storable.Container == null
			    ? intersect.Storage.AddItem(intersect.Storable, intersect.Container)
			    : ((StorageComponent)storable.Storage).MoveItem(storable, intersect.Storage, intersect.Container,
				    intersect.Cell.To())))
			throw new Exception();
	}

	public IEnumerator Load(IErrorLoadingHandler errorHandler) {
		yield break;
	}

	public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler) {
		var node = element[TypeUtility.GetTypeName(GetType())];
		if (node == null)
			errorHandler.LogError(TypeUtility.GetTypeName(GetType()) + " node not found , context : " + context);
		else {
			var reader = new XmlNodeDataReader(node, context);
			((ISerializeStateLoad)this).StateLoad(reader, GetType());
			yield break;
		}
	}

	public void Unload() {
		bags.Clear();
	}

	public void Save(IDataWriter writer, string context) {
		DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
	}
}
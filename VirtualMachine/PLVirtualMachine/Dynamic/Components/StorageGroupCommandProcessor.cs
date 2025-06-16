using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

public class StorageGroupCommandProcessor :
	StorageCommandProcessor,
	ISerializeStateSave,
	IDynamicLoadSerializable {
	private List<StorageCommand> loadedCommandList = new();

	public override void Clear() {
		base.Clear();
		if (loadedCommandList == null)
			return;
		foreach (var loadedCommand in loadedCommandList)
			loadedCommand.Clear();
		loadedCommandList.Clear();
		loadedCommandList = null;
	}

	public int ProcessRandomAddItemsToStorage(StorageCommand storageCommand) {
		var template =
			ServiceCache.TemplateService.GetTemplate<IEntity>(storageCommand.TargetItemTemplate.EngineTemplateGuid);
		if (template != null) {
			var storableComponent = VMStorable.MakeStorableByTemplate(storageCommand.TargetStorage.Parent, template);
			var owner = storableComponent.Owner;
			var itemsToAdd = 1;
			if (storageCommand.ItemsCount > 1) {
				itemsToAdd = (int)Math.Round(storageCommand.ItemsCount * (2.0 * VMMath.GetRandomDouble()));
				if (itemsToAdd >= storableComponent.Max)
					itemsToAdd = storableComponent.Max;
			}

			if (itemsToAdd > storageCommand.MaxItemsCount)
				itemsToAdd = storageCommand.MaxItemsCount;
			if (DoAddItemsToStorage(storageCommand.TargetStorage.Parent.Instance, owner, itemsToAdd))
				return itemsToAdd;
			owner.Dispose();
		}

		return 0;
	}

	public void StateSave(IDataWriter writer) {
		SaveManagerUtility.SaveDynamicSerializableList(writer, "StorageGroupCommandQueue", commandsQueue);
	}

	public void LoadFromXML(XmlElement xmlNode) {
		loadedCommandList.Clear();
		for (var i = 0; i < xmlNode.ChildNodes.Count; ++i)
			if (xmlNode.ChildNodes[i].Name == "StorageGroupCommandQueue")
				VMSaveLoadManager.LoadDynamiSerializableList((XmlElement)xmlNode.ChildNodes[i], loadedCommandList);
	}

	public void AfterSaveLoading() {
		commandsQueue.Clear();
		if (loadedCommandList.Count > 0)
			for (var index = 0; index < loadedCommandList.Count; ++index) {
				var loadedCommand = loadedCommandList[index];
				loadedCommand.AfterSaveLoading();
				commandsQueue.Enqueue(loadedCommand);
			}

		Active = commandsQueue.Count > 0;
	}

	protected override void ProcessCommand(IStorageCommand storageCommand) {
		var storageCommand1 = (StorageCommand)storageCommand;
		if (storageCommand.StorageCommandType == EStorageCommandType.StorageCommandTypeAddItem) {
			if (storageCommand1.ItemsCount > 1 || storageCommand1.CombinationParams != null)
				DoAddItemstoStorage(storageCommand1.TargetStorage, storageCommand1.TargetItemTemplate,
					storageCommand1.ItemsCount, storageCommand1.ContainerTypesInfo, storageCommand1.ContainerTagsInfo,
					storageCommand1.CombinationParams, storageCommand1.DropIfBusyMode);
			else
				DoAddItemstoStorage(storageCommand1.TargetStorage, storageCommand1.TargetItemTemplate,
					storageCommand1.ContainerTypesInfo, storageCommand1.ContainerTagsInfo,
					storageCommand1.DropIfBusyMode);
		} else {
			if (storageCommand.StorageCommandType != EStorageCommandType.StorageCommandClear)
				return;
			DoFreeStorage(storageCommand1.TargetStorage);
		}
	}

	private void DoFreeStorage(VMStorage targetStorage) {
		targetStorage.Component.ClearItems();
	}

	private void DoAddItemstoStorage(
		VMStorage storage,
		VMWorldObject itemLogicObject,
		OperationTagsInfo containerTypes,
		OperationMultiTagsInfo containerTags,
		bool dropIfBusy = false) {
		var template = ServiceCache.TemplateService.GetTemplate<IEntity>(itemLogicObject.EngineTemplateGuid);
		var storableComponent = VMStorable.MakeStorableByTemplate(storage.Parent, template);
		var owner = storableComponent.Owner;
		var itemsToAdd = 1;
		if (DoAddItemsToStorage(storage, owner, itemsToAdd, containerTypes, containerTags, 0, dropIfBusy: dropIfBusy))
			return;
		storableComponent.Owner.Dispose();
	}

	private void DoAddItemstoStorage(
		VMStorage storage,
		VMWorldObject itemLogicObject,
		int itemsToAdd,
		OperationTagsInfo containerTypes,
		OperationMultiTagsInfo containerTags,
		CombinationItemParams ciParams = null,
		bool dropIfBusy = false) {
		if (storage.Parent.IsDisposed)
			return;
		var template = ServiceCache.TemplateService.GetTemplate<IEntity>(itemLogicObject.EngineTemplateGuid);
		template.GetComponent<IStorableComponent>();
		var num = 0;
		var needContainerNum = 0;
		do {
			var storableComponent = VMStorable.MakeStorableByTemplate(storage.Parent, template);
			if (storableComponent != null) {
				var owner = storableComponent.Owner;
				if (owner != null) {
					var itemsToAdd1 = storableComponent.Max;
					if (itemsToAdd1 > itemsToAdd - num)
						itemsToAdd1 = itemsToAdd - num;
					if (!DoAddItemsToStorage(storage, owner, itemsToAdd1, containerTypes, containerTags,
						    needContainerNum, ciParams, dropIfBusy))
						owner.Dispose();
					num += itemsToAdd1;
				}

				++needContainerNum;
			} else {
				Logger.AddError(string.Format("Cannot create storable by template {0} in storage {1} at {2}",
					template.Name, storage.Parent.Name, DynamicFSM.CurrentStateInfo));
				break;
			}
		} while (num < itemsToAdd);
	}

	private bool DoAddItemsToStorage(
		VMStorage storage,
		IEntity addingItemEntity,
		int itemsToAdd,
		OperationTagsInfo containerTypes,
		OperationMultiTagsInfo containerTags,
		int needContainerNum,
		CombinationItemParams combItemParams = null,
		bool dropIfBusy = false) {
		if (storage == null) {
			Logger.AddError(string.Format("Storage for adding items not defined at {0}", DynamicFSM.CurrentStateInfo));
			return false;
		}

		if (addingItemEntity == null) {
			Logger.AddError(string.Format("Adding item entity not defined at {0}", DynamicFSM.CurrentStateInfo));
			return false;
		}

		var component1 = addingItemEntity.GetComponent<IStorableComponent>();
		if (component1 == null) {
			Logger.AddError(string.Format(
				"Add item to storage error: adding item Entity {0} hasn't storable component at {1}",
				addingItemEntity.Name, DynamicFSM.CurrentStateInfo));
			return false;
		}

		if (component1.Owner == null) {
			Logger.AddError(string.Format(
				"Add item to storage error: adding item Entity {0} storable component is invalid at {1}",
				addingItemEntity.Name, DynamicFSM.CurrentStateInfo));
			return false;
		}

		var storage1 = false;
		if (itemsToAdd > 0) {
			component1.Count = itemsToAdd;
			try {
				var component2 = storage.Component;
				var flag1 = false;
				if (containerTypes != null && containerTypes.TagsList.Count > 0)
					flag1 = true;
				var flag2 = false;
				if (containerTags != null && containerTags.TagsList.Count > 0)
					flag2 = true;
				if (flag1 | flag2) {
					var containersByTagsList = storage.GetInnerContainersByTagsList(containerTags);
					var num1 = 0;
					var num2 = 0;
					do {
						for (var index = 0; index < containersByTagsList.Count; ++index) {
							var flag3 = true;
							if (flag1)
								flag3 = containerTypes.CheckTag(containersByTagsList[index].Owner.Name);
							if (flag3) {
								if (num1 >= needContainerNum || num2 > 0) {
									if (VMStorage.DoAddItemToStorage(component2, component1,
										    containersByTagsList[index], dropIfBusy)) {
										storage1 = true;
										break;
									}
								} else
									++num1;
							}
						}

						++num2;
						if (storage1)
							break;
					} while (num2 < 2);
				} else
					storage1 = VMStorage.DoAddItemToStorage(component2, component1, dropIfBusy: dropIfBusy);
			} catch (Exception ex) {
				Logger.AddError(ex.ToString());
			}

			if (storage1 && combItemParams != null) {
				var minIntVal = 1000 * combItemParams.MinDurablityProc;
				var maxIntVal = 1000 * combItemParams.MaxDurablityProc;
				if (maxIntVal < minIntVal)
					maxIntVal = minIntVal;
				var randomInt = VMMath.GetRandomInt(minIntVal, maxIntVal);
				component1.Durability.Value = 1E-05f * randomInt;
			}
		} else
			Logger.AddWarning(string.Format("Add item to storage warning: adding items '{0}' count is 0 at {1}",
				addingItemEntity.Name, DynamicFSM.CurrentStateInfo));

		return storage1;
	}

	private bool DoAddItemsToStorage(
		IEntity storageEntity,
		IEntity addingItemEntity,
		int itemsToAdd) {
		if (storageEntity == null) {
			Logger.AddError("Storage entity for adding items not defined at " + DynamicFSM.CurrentStateInfo);
			return false;
		}

		if (addingItemEntity == null) {
			Logger.AddError("Adding item entity not defined at " + DynamicFSM.CurrentStateInfo);
			return false;
		}

		var component1 = storageEntity.GetComponent<IStorageComponent>();
		var component2 = addingItemEntity.GetComponent<IStorableComponent>();
		if (component2 == null) {
			Logger.AddError(string.Format(
				"Add item to storage error: adding item Entity {0} hasn't storable component at {1}",
				addingItemEntity.Name, DynamicFSM.CurrentStateInfo));
			return false;
		}

		if (component2.Owner == null) {
			Logger.AddError(string.Format(
				"Add item to storage error: adding item Entity {0} storable component is invalid at {1}",
				addingItemEntity.Name, DynamicFSM.CurrentStateInfo));
			return false;
		}

		component2.Count = itemsToAdd;
		return VMStorage.DoAddItemToStorage(component1, component2);
	}
}
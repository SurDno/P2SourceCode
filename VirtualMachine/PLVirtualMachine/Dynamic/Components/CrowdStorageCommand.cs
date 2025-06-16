using System;
using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Dynamic.Components;

public class CrowdStorageCommand : IStorageCommand {
	private Guid targetStorageEntityLoadedId = Guid.Empty;

	public void Initialize(
		EStorageCommandType commandType,
		VMStorage storage,
		IEntity containerEntity,
		[Template] IEntity template,
		int needCount,
		bool dropIfBusy = false) {
		StorageCommandType = commandType;
		TargetStorage = storage;
		TargetContainer = containerEntity;
		TargetItemTemplate = template;
		NeedItemsCount = needCount;
		DropIfBusyMode = dropIfBusy;
	}

	public void Clear() {
		TargetStorage = null;
		TargetItemTemplate = null;
		TargetContainer = null;
	}

	public EStorageCommandType StorageCommandType { get; private set; }

	public VMStorage TargetStorage { get; private set; }

	public IEntity TargetContainer { get; private set; }

	public IEntity TargetItemTemplate { get; private set; }

	public int NeedItemsCount { get; private set; }

	public bool DropIfBusyMode { get; private set; }
}
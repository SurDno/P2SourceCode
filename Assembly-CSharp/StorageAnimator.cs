using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Connections;
using UnityEngine;
using UnityEngine.Serialization;

public class StorageAnimator : MonoBehaviour, IEntityAttachable {
	[SerializeField] [FormerlySerializedAs("infos")]
	private List<ContainerInfo> containers = new();

	private StorageComponent storage;

	public void Attach(IEntity owner) {
		storage = owner.GetComponent<StorageComponent>();
		if (storage == null)
			return;
		storage.ChangeInventoryEvent += ChangeInventoryEvent;
		ComputeContainers();
	}

	public void Detach() {
		if (storage == null)
			return;
		storage.ChangeInventoryEvent -= ChangeInventoryEvent;
		storage = null;
	}

	private void ChangeInventoryEvent(IStorageComponent storage, IInventoryComponent container) {
		ComputeContainers();
	}

	private void ComputeContainers() {
		foreach (var container1 in storage.Containers) {
			var container = container1;
			var containerInfo = containers.FirstOrDefault(o => {
				if (container.Owner.TemplateId == o.EntityContainer.Id)
					return true;
				return container.Owner.Template != null && container.Owner.Template.TemplateId == o.EntityContainer.Id;
			});
			if (containerInfo != null)
				foreach (var animator in containerInfo.Animators)
					if (animator != null)
						animator.IsOpened = container.OpenState.Value == ContainerOpenStateEnum.Open;
		}
	}

	[Serializable]
	public class ContainerInfo {
		public IEntitySerializable EntityContainer;
		public ContainerAnimator[] Animators = new ContainerAnimator[0];
	}
}
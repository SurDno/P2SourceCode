using System;
using System.Collections.Generic;
using Engine.Common.Components.Parameters;

namespace Engine.Common.Components;

public interface IStorageComponent : IComponent {
	string Tag { get; }

	IParameterValue<bool> IsFree { get; }

	IEnumerable<IStorableComponent> Items { get; }

	IEnumerable<IInventoryComponent> Containers { get; }

	IEnumerable<IEntity> InventoryTemplates { get; }

	event Action<IStorableComponent, IInventoryComponent> OnAddItemEvent;

	event Action<IStorableComponent, IInventoryComponent> OnRemoveItemEvent;

	event Action<IStorableComponent, IInventoryComponent> OnChangeItemEvent;

	void AddItemOrDrop(IStorableComponent item, IInventoryComponent container);

	bool AddItem(IStorableComponent item, IInventoryComponent container);

	bool MoveItem(
		IStorableComponent item,
		IStorageComponent storage,
		IInventoryComponent container);

	bool RemoveItem(IStorableComponent item);

	void ClearItems(IInventoryComponent inventory);

	void ClearItems();
}
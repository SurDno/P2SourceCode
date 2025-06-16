// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IStorageComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Components.Parameters;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Components
{
  public interface IStorageComponent : IComponent
  {
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
}

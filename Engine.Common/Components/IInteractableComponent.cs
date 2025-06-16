// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IInteractableComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Components.Interactable;
using Engine.Common.Types;
using System;

#nullable disable
namespace Engine.Common.Components
{
  public interface IInteractableComponent : IComponent
  {
    bool IsEnabled { get; set; }

    LocalizedText Title { get; set; }

    event Action<IEntity, IInteractableComponent, IInteractItem> BeginInteractEvent;

    event Action<IEntity, IInteractableComponent, IInteractItem> EndInteractEvent;
  }
}

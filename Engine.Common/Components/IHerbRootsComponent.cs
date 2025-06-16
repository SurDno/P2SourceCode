// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IHerbRootsComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;

#nullable disable
namespace Engine.Common.Components
{
  public interface IHerbRootsComponent : IComponent
  {
    void Reset();

    event Action OnTriggerEnterEvent;

    event Action OnTriggerLeaveEvent;

    event Action OnActivateStartEvent;

    event Action OnActivateEndEvent;

    event Action OnHerbSpawnEvent;

    event Action OnLastHerbSpawnEvent;
  }
}

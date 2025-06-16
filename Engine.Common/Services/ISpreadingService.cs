// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.ISpreadingService
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Commons;
using Engine.Common.Components.Regions;
using System;

#nullable disable
namespace Engine.Common.Services
{
  public interface ISpreadingService
  {
    event Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> OnFurnitureLoadedOnce;

    event Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> OnFurnitureLoaded;

    event Action<IEntity> OnRegionLoadedOnce;

    event Action<IEntity> OnRegionLoaded;

    void Reset();
  }
}

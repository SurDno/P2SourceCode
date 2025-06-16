// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.ISimulation
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;

#nullable disable
namespace Engine.Common.Services
{
  public interface ISimulation
  {
    IEntity Hierarchy { get; }

    IEntity Objects { get; }

    IEntity Storables { get; }

    IEntity Others { get; }

    IEntity Player { get; }

    IEntity Get(Guid id);

    void Add(IEntity entity, IEntity parent);
  }
}

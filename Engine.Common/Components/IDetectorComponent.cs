// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IDetectorComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Components
{
  public interface IDetectorComponent : IComponent
  {
    bool IsEnabled { get; set; }

    HashSet<IDetectableComponent> Visible { get; }

    HashSet<IDetectableComponent> Hearing { get; }

    event Action<IDetectableComponent> OnSee;

    event Action<IDetectableComponent> OnStopSee;

    event Action<IDetectableComponent> OnHear;
  }
}

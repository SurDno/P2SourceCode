// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IBlueprintComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Commons;
using System;

#nullable disable
namespace Engine.Common.Components
{
  public interface IBlueprintComponent : IComponent
  {
    event Action<IBlueprintComponent> CompleteEvent;

    event Action<IBlueprintComponent> AttachEvent;

    IBlueprintObject Blueprint { get; set; }

    bool IsStarted { get; }

    bool IsAttached { get; }

    void Start();

    void Start(IEntity owner);

    void Stop();

    void SendEvent(string name);
  }
}

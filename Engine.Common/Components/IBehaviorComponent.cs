// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IBehaviorComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Commons;
using System;

#nullable disable
namespace Engine.Common.Components
{
  public interface IBehaviorComponent : IComponent
  {
    event Action<IBehaviorComponent> SuccessEvent;

    event Action<IBehaviorComponent> FailEvent;

    event Action<IBehaviorComponent, string> CustomEvent;

    IBehaviorObject BehaviorObject { get; set; }

    IBehaviorObject BehaviorObjectForced { get; set; }

    void SetValue(string name, IEntity value);

    void SetBoolValue(string name, bool value);

    void SetIntValue(string name, int value);

    void SetFloatValue(string name, float value);

    void SetBehaviorForced(IBehaviorObject behaviorObject);
  }
}

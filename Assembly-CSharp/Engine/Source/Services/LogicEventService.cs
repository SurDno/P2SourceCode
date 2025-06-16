// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.LogicEventService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using System;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (LogicEventService), typeof (ILogicEventService)})]
  public class LogicEventService : ILogicEventService
  {
    public event Action<string> OnCommonEvent;

    public event Action<string, string> OnValueEvent;

    public event Action<string, IEntity> OnEntityEvent;

    public void FireCommonEvent(string name)
    {
      Action<string> onCommonEvent = this.OnCommonEvent;
      if (onCommonEvent == null)
        return;
      onCommonEvent(name);
    }

    public void FireValueEvent(string name, string value)
    {
      Action<string, string> onValueEvent = this.OnValueEvent;
      if (onValueEvent == null)
        return;
      onValueEvent(name, value);
    }

    public void FireEntityEvent(string name, IEntity entity)
    {
      Action<string, IEntity> onEntityEvent = this.OnEntityEvent;
      if (onEntityEvent == null)
        return;
      onEntityEvent(name, entity);
    }
  }
}

﻿using System;
using Engine.Common;
using Engine.Common.Services;

namespace Engine.Source.Services
{
  [GameService(typeof (LogicEventService), typeof (ILogicEventService))]
  public class LogicEventService : ILogicEventService
  {
    public event Action<string> OnCommonEvent;

    public event Action<string, string> OnValueEvent;

    public event Action<string, IEntity> OnEntityEvent;

    public void FireCommonEvent(string name)
    {
      Action<string> onCommonEvent = OnCommonEvent;
      if (onCommonEvent == null)
        return;
      onCommonEvent(name);
    }

    public void FireValueEvent(string name, string value)
    {
      Action<string, string> onValueEvent = OnValueEvent;
      if (onValueEvent == null)
        return;
      onValueEvent(name, value);
    }

    public void FireEntityEvent(string name, IEntity entity)
    {
      Action<string, IEntity> onEntityEvent = OnEntityEvent;
      if (onEntityEvent == null)
        return;
      onEntityEvent(name, entity);
    }
  }
}

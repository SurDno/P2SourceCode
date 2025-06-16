using Engine.Common;
using Engine.Common.Services;
using System;

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

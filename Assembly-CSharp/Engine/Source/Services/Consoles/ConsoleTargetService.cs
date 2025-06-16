using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using System;
using System.Collections.Generic;

namespace Engine.Source.Services.Consoles
{
  public static class ConsoleTargetService
  {
    private static Dictionary<string, Func<string, object>> targets = new Dictionary<string, Func<string, object>>();

    public static void AddTarget(string name, Func<string, object> func)
    {
      ConsoleTargetService.targets.Add(name, func);
    }

    public static bool GetTarget(string name, string value, out object result)
    {
      result = (object) null;
      Func<string, object> func;
      if (name.IsNullOrEmpty() || !ConsoleTargetService.targets.TryGetValue(name, out func))
        return false;
      result = func(value);
      return true;
    }

    public static object GetTarget(Type type, ConsoleParameter parameter)
    {
      object result;
      if (ConsoleTargetService.GetTarget(type.Name, "", out result) || ConsoleTargetService.GetTarget(parameter.Parameter, parameter.Value, out result))
        return ConsoleTargetService.GetTarget(type, result);
      return parameter.Parameter.IsNullOrEmpty() ? ConsoleTargetService.GetTarget(type, (object) ServiceLocator.GetService<ISimulation>().Player) : (object) null;
    }

    public static object GetTarget(Type type, object target)
    {
      if (target == null)
        return (object) null;
      if (TypeUtility.IsAssignableFrom(type, target.GetType()))
        return target;
      if (target is IEntity entity)
      {
        if (typeof (IComponent).IsAssignableFrom(type))
          return (object) entity.GetComponent(type);
      }
      else if (target is IComponent component)
      {
        if (typeof (IEntity).IsAssignableFrom(type))
          return (object) component.Owner;
        if (typeof (IComponent).IsAssignableFrom(type))
          return (object) entity.GetComponent(type);
      }
      return (object) null;
    }
  }
}

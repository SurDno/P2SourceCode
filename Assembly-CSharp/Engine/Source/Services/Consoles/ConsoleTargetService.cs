﻿using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;

namespace Engine.Source.Services.Consoles
{
  public static class ConsoleTargetService
  {
    private static Dictionary<string, Func<string, object>> targets = new();

    public static void AddTarget(string name, Func<string, object> func)
    {
      targets.Add(name, func);
    }

    public static bool GetTarget(string name, string value, out object result)
    {
      result = null;
      if (name.IsNullOrEmpty() || !targets.TryGetValue(name, out Func<string, object> func))
        return false;
      result = func(value);
      return true;
    }

    public static object GetTarget(Type type, ConsoleParameter parameter)
    {
	    if (GetTarget(type.Name, "", out object result) || GetTarget(parameter.Parameter, parameter.Value, out result))
        return GetTarget(type, result);
      return parameter.Parameter.IsNullOrEmpty() ? GetTarget(type, ServiceLocator.GetService<ISimulation>().Player) : null;
    }

	public static object GetTarget(Type type, object target)
	{
	    if (target == null)
	        return null;
	    if (TypeUtility.IsAssignableFrom(type, target.GetType()))
	        return target;
	    if (target is IEntity entity)
	    {
	        if (typeof(IComponent).IsAssignableFrom(type))
	            return entity.GetComponent(type);
	    }
	    else if (target is IComponent component)
	    {
	        if (typeof(IEntity).IsAssignableFrom(type))
	            return component.Owner;
	        if (typeof(IComponent).IsAssignableFrom(type))
	            return component.Owner.GetComponent(type);
	    }
	    return null;
	}
  }
}

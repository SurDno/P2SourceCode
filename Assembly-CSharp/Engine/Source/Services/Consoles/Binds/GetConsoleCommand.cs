using Cofe.Meta;
using Cofe.Serializations.Converters;
using System;
using System.Collections.Generic;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class GetConsoleCommand
  {
    private static Dictionary<string, GetConsoleCommand.Holder> binds = new Dictionary<string, GetConsoleCommand.Holder>();

    public static void AddBind(
      Type type,
      Type valueType,
      string name,
      bool needTarget,
      Func<object, object> action)
    {
      GetConsoleCommand.Holder holder = new GetConsoleCommand.Holder()
      {
        Type = type,
        Func = (Func<object, string>) (target =>
        {
          if (needTarget)
          {
            target = ConsoleTargetService.GetTarget(type, target);
            if (target == null)
              return "Error, target not found : " + (object) type + " : " + name;
          }
          if (valueType.IsEnum)
            return DefaultConverter.ToString((Enum) target);
          if (!ConvertService.ContainsConverter(valueType))
            return "Error, parser type not found : " + (object) valueType + " : " + name;
          string result;
          if (ConvertService.ToString(valueType, action(target), out result))
            return result;
          return "Error to string value : " + (object) valueType + " : " + name;
        })
      };
      GetConsoleCommand.binds.Add(name, holder);
    }

    public static void AddBind<TObject, T>(string name, bool needTarget, Func<TObject, T> action) where TObject : class
    {
      GetConsoleCommand.AddBind(typeof (TObject), typeof (T), name, needTarget, (Func<object, object>) (target => (object) action(target as TObject)));
    }

    [ConsoleCommand("get")]
    private static string Command(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
      {
        string str = command + " [target] property\n\nProperties :\n";
        foreach (KeyValuePair<string, GetConsoleCommand.Holder> bind in GetConsoleCommand.binds)
          str = str + bind.Key + "\n";
        return str;
      }
      GetConsoleCommand.Holder holder;
      object target;
      if (parameters.Length == 1)
      {
        if (!GetConsoleCommand.binds.TryGetValue(parameters[0].Value, out holder))
          return "Parameter not found";
        target = ConsoleTargetService.GetTarget(holder.Type, new ConsoleParameter());
      }
      else
      {
        if (parameters.Length != 2)
          return "Error parameter count";
        if (!GetConsoleCommand.binds.TryGetValue(parameters[1].Value, out holder))
          return "Parameter not found";
        target = ConsoleTargetService.GetTarget(holder.Type, parameters[0]);
      }
      return holder.Func(target);
    }

    private class Holder
    {
      public Type Type;
      public Func<object, string> Func;
    }
  }
}

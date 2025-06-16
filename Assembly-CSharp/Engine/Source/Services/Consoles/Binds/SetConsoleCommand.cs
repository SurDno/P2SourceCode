using Cofe.Meta;
using Cofe.Serializations.Converters;
using System;
using System.Collections.Generic;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class SetConsoleCommand
  {
    private static Dictionary<string, SetConsoleCommand.Holder> binds = new Dictionary<string, SetConsoleCommand.Holder>();

    public static void AddBind(
      Type type,
      Type valueType,
      string name,
      bool needTarget,
      Action<object, object> action)
    {
      SetConsoleCommand.Holder holder = new SetConsoleCommand.Holder()
      {
        Type = type,
        Func = (Func<object, string, string>) ((target, value) =>
        {
          object result1;
          if (valueType.IsEnum)
          {
            Enum result2;
            if (!DefaultConverter.TryParseEnum(value, valueType, out result2))
              return "Error parse value : \"" + value + "\" , name : " + name;
            result1 = (object) result2;
          }
          else
          {
            if (!ConvertService.ContainsConverter(valueType))
              return "Error, parser type not found : " + (object) valueType + " , name : " + name;
            if (!ConvertService.TryParse(valueType, value, out result1))
              return "Error parse value : \"" + value + "\" , name : " + name;
          }
          if (needTarget)
          {
            target = ConsoleTargetService.GetTarget(type, target);
            if (target == null)
              return "Error, target not found : " + (object) type + " , name : " + name;
          }
          action(target, result1);
          return "Change " + name + " to : " + value;
        })
      };
      SetConsoleCommand.binds.Add(name, holder);
    }

    public static void AddBind<TObject, T>(string name, bool needTarget, Action<TObject, T> action) where TObject : class
    {
      SetConsoleCommand.AddBind(typeof (TObject), typeof (T), name, needTarget, (Action<object, object>) ((target, value) => action(target as TObject, (T) value)));
    }

    [ConsoleCommand("set")]
    private static string Command(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
      {
        string str = command + " [target] property value\n\nProperties :\n";
        foreach (KeyValuePair<string, SetConsoleCommand.Holder> bind in SetConsoleCommand.binds)
          str = str + bind.Key + "\n";
        return str;
      }
      SetConsoleCommand.Holder holder;
      object target;
      string str1;
      if (parameters.Length == 2)
      {
        if (!SetConsoleCommand.binds.TryGetValue(parameters[0].Value, out holder))
          return "Parameter not found";
        target = ConsoleTargetService.GetTarget(holder.Type, new ConsoleParameter());
        str1 = parameters[1].Value;
      }
      else
      {
        if (parameters.Length != 3)
          return "Error parameter count";
        if (!SetConsoleCommand.binds.TryGetValue(parameters[1].Value, out holder))
          return "Parameter not found";
        target = ConsoleTargetService.GetTarget(holder.Type, parameters[0]);
        str1 = parameters[2].Value;
      }
      return holder.Func(target, str1);
    }

    private class Holder
    {
      public Type Type;
      public Func<object, string, string> Func;
    }
  }
}

using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class EnumConsoleCommand
  {
    private static Dictionary<string, Func<string>> binds = new Dictionary<string, Func<string>>();

    public static void AddBind(string name, Func<string> func)
    {
      binds.Add(name, (Func<string>) (() => func()));
    }

    [ConsoleCommand("enum")]
    private static string Command(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
      {
        string str = command + " target \n\nTargets :\n";
        foreach (KeyValuePair<string, Func<string>> bind in binds)
          str = str + bind.Key + "\n";
        return str;
      }
      if (parameters.Length != 1)
        return "Error parameter count";
      Func<string> func;
      binds.TryGetValue(parameters[0].Parameter, out func);
      return func == null ? "Target not found" : func();
    }
  }
}

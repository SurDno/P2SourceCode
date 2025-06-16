// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.EnumConsoleCommand
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class EnumConsoleCommand
  {
    private static Dictionary<string, Func<string>> binds = new Dictionary<string, Func<string>>();

    public static void AddBind(string name, Func<string> func)
    {
      EnumConsoleCommand.binds.Add(name, (Func<string>) (() => func()));
    }

    [ConsoleCommand("enum")]
    private static string Command(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
      {
        string str = command + " target \n\nTargets :\n";
        foreach (KeyValuePair<string, Func<string>> bind in EnumConsoleCommand.binds)
          str = str + bind.Key + "\n";
        return str;
      }
      if (parameters.Length != 1)
        return "Error parameter count";
      Func<string> func;
      EnumConsoleCommand.binds.TryGetValue(parameters[0].Parameter, out func);
      return func == null ? "Target not found" : func();
    }
  }
}

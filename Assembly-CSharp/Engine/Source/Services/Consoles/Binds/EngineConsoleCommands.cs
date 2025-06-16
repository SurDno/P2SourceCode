using System;
using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common.Services;
using Engine.Source.Services.CameraServices;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class EngineConsoleCommands
  {
    [ConsoleCommand("camera")]
    private static string CameraCommand(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length > 1 || parameters.Length == 1 && parameters[0].Value == "?")
      {
        string str = "";
        foreach (string name in Enum.GetNames(typeof (CameraKindEnum)))
        {
          if (str != "")
            str += " | ";
          str += name;
        }
        return command + " [" + str + "]";
      }
      CameraKindEnum cameraKindEnum = DefaultConverter.ParseEnum<CameraKindEnum>(parameters[0].Value);
      ServiceLocator.GetService<CameraService>().Kind = cameraKindEnum;
      return "Set Camera : " + cameraKindEnum;
    }

    [ConsoleCommand("event")]
    private static string EventCommand(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length > 2 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " name [value]";
      string str1;
      if (parameters.Length == 1)
      {
        string name = parameters[0].Value;
        ServiceLocator.GetService<LogicEventService>().FireCommonEvent(name);
        str1 = "event : \"" + name + "\"";
      }
      else
      {
        if (parameters.Length != 2)
          return "Error parameter count";
        string name = parameters[0].Value;
        string str2 = parameters[1].Value;
        ServiceLocator.GetService<LogicEventService>().FireValueEvent(name, str2);
        str1 = "event : \"" + name + "\" " + str2;
      }
      return str1;
    }
  }
}

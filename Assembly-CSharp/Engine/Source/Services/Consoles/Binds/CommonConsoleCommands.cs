using System.Collections;
using System.IO;
using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common.Services;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class CommonConsoleCommands
  {
    [ConsoleCommand("test")]
    private static string TestCommand(string command, ConsoleParameter[] parameters)
    {
      string str = "Command : \"" + command + "\"\n\n";
      for (int index = 0; index < parameters.Length; ++index)
      {
        ConsoleParameter parameter = parameters[index];
        str = str + "Parameter : \"" + parameter.Parameter + "\"\n" + "Value : \"" + parameter.Value + "\"\n\n";
      }
      return str;
    }

    [ConsoleCommand("print")]
    private static string PrintCommand(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length > 1 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " filename";
      return !File.Exists(parameters[0].Value) ? "file not found : " + parameters[0].Value : File.ReadAllText(parameters[0].Value);
    }

    [ConsoleCommand("exec")]
    private static string ExecCommand(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length > 1 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " filename";
      string str1 = "Data/ConsoleCommands/";
      string str2 = ".txt";
      string path = parameters[0].Value;
      if (!File.Exists(path))
      {
        if (File.Exists(parameters[0].Value + str2))
          path = parameters[0].Value + str2;
        else if (File.Exists(str1 + parameters[0].Value))
        {
          path = str1 + parameters[0].Value;
        }
        else
        {
          if (!File.Exists(str1 + parameters[0].Value + str2))
            return "file not found : " + parameters[0].Value;
          path = str1 + parameters[0].Value + str2;
        }
      }
      CoroutineService.Instance.Route(ExecuteCommands(File.ReadAllLines(path)));
      return "";
    }

    private static IEnumerator ExecuteCommands(string[] lines)
    {
      ConsoleService console = ServiceLocator.GetService<ConsoleService>();
      string gotoTag = "#goto";
      string delayTag = "#delay";
      string labelTag = ":";
      for (int index = 0; index < lines.Length; ++index)
      {
        string line = lines[index];
        if (line == "")
          yield return null;
        else if (line.StartsWith("//"))
          yield return null;
        else if (line.StartsWith(":"))
          yield return null;
        else if (line.StartsWith(gotoTag))
        {
          string valueText = line.Substring(gotoTag.Length).Trim();
          int num = lines.IndexOf(labelTag + valueText);
          if (num == -1)
          {
            console.AddLine("error: label '" + valueText + "' not found");
            break;
          }
          index = num - 1;
          console.AddLine(line);
          yield return null;
        }
        else if (line.StartsWith(delayTag))
        {
          string valueText = line.Substring(delayTag.Length).Trim();
          float value = DefaultConverter.ParseFloat(valueText);
          console.AddLine(line);
          yield return (object) new WaitForSecondsRealtime(value);
        }
        else
        {
          console.ExecuteCommand(line);
          yield return null;
          line = null;
        }
      }
    }
  }
}

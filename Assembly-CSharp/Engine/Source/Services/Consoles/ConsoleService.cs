﻿using System;
using System.Collections.Generic;
using Cofe.Utility;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services.Consoles
{
  [RuntimeService(typeof (ConsoleService))]
  public class ConsoleService
  {
    public const string HelpToken = "?";
    public const char ParameterToken = '-';
    public const string ParameterTokenText = "-";
    public const char DelimiterToken = ':';
    public const string DelimiterTokenText = ":";
    public const char SpaceToken = ' ';
    public const string NewLineToken = "\n";
    public const char QuoteToken = '"';
    public const string CommandNotFoundText = "Command not found : ";
    public const string CommonHelpText = "Format :\ncommand [value | \"value with spaces\" | -param | -param: value | -param: \"value with spaces\"]\n\nCommands :\n";
    public const string FirstHelpText = "Print \"?\" for help";
    public const string PrefixExecuteCommandText = "Execute command : ";
    private Action<string> onAddedLine;
    private object sync = new();
    [Inspected]
    private static Dictionary<string, Func<string, ConsoleParameter[], string>> commands = new();

    public event Action<string> OnAddedLine
    {
      add
      {
        lock (sync)
        {
          if (onAddedLine == null && value != null)
            value("Print \"?\" for help");
          onAddedLine += value;
        }
      }
      remove
      {
        lock (sync)
          onAddedLine -= value;
      }
    }

    public static void RegisterCommand(string name, Func<string, ConsoleParameter[], string> func)
    {
      if (commands.ContainsKey(name))
        Debug.LogError("Command exist : " + name);
      else
        commands.Add(name, func);
    }

    public void ExecuteCommand(string value)
    {
      if (value == "?")
      {
        AddLine(GetHelpText());
      }
      else
      {
        string str = "Execute command : \"" + value + "\"\n" + ComputeCommand(value);
        char[] chArray = ['\n'];
        foreach (string line in str.Split(chArray))
          AddLine(line);
      }
    }

    public void AddLine(string line)
    {
      Action<string> onAddedLine = this.onAddedLine;
      if (onAddedLine == null)
        return;
      onAddedLine(line);
    }

    private string GetHelpText()
    {
      string helpText = "Format :\ncommand [value | \"value with spaces\" | -param | -param: value | -param: \"value with spaces\"]\n\nCommands :\n";
      foreach (string key in commands.Keys)
        helpText = helpText + key + "\n";
      return helpText;
    }

    private string ComputeCommand(string value)
    {
      value = value.Trim();
      int num = value.IndexOf(' ');
      string key;
      if (num == -1)
      {
        key = value;
        value = "";
      }
      else
      {
        key = value.Substring(0, num);
        value = value.Substring(num);
      }

      if (!commands.TryGetValue(key, out Func<string, ConsoleParameter[], string> func))
        return "Command not found : " + key;
      List<ConsoleParameter> parameters = GetParameters(value);
      return func(key, parameters.ToArray());
    }

    private List<ConsoleParameter> GetParameters(string text)
    {
      text = text.Trim();
      List<ConsoleParameter> parameters = [];
      List<string> stringList = [];
      int startIndex = 0;
      bool flag = false;
      for (int index = 0; index < text.Length; ++index)
      {
        char ch = text[index];
        if (ch == '"')
        {
          if (flag)
          {
            string str = text.Substring(startIndex, index - startIndex + 1).Trim();
            if (!str.IsNullOrEmpty())
              stringList.Add(str);
            startIndex = index + 1;
            flag = false;
          }
          else
          {
            startIndex = index;
            flag = true;
          }
        }
        if (!flag && (ch == ' ' || ch == ':'))
        {
          string str = text.Substring(startIndex, index - startIndex + 1).Trim();
          if (!str.IsNullOrEmpty())
          {
            stringList.Add(str);
            startIndex = index + 1;
          }
        }
      }
      string str1 = text.Substring(startIndex).Trim();
      if (!str1.IsNullOrEmpty())
        stringList.Add(str1);
      for (int index = 0; index < stringList.Count; ++index)
      {
        string str2 = stringList[index];
        ConsoleParameter consoleParameter1;
        if (str2.StartsWith("-"))
        {
          if (str2.EndsWith(":"))
          {
            if (index + 1 < stringList.Count)
            {
              List<ConsoleParameter> consoleParameterList = parameters;
              consoleParameter1 = new ConsoleParameter();
              consoleParameter1.Parameter = str2.Trim(':');
              consoleParameter1.Value = stringList[index + 1].Trim(' ', '"');
              ConsoleParameter consoleParameter2 = consoleParameter1;
              consoleParameterList.Add(consoleParameter2);
              ++index;
            }
            else
            {
              List<ConsoleParameter> consoleParameterList = parameters;
              consoleParameter1 = new ConsoleParameter();
              consoleParameter1.Parameter = str2.Trim(':');
              consoleParameter1.Value = "";
              ConsoleParameter consoleParameter3 = consoleParameter1;
              consoleParameterList.Add(consoleParameter3);
            }
          }
          else
          {
            List<ConsoleParameter> consoleParameterList = parameters;
            consoleParameter1 = new ConsoleParameter();
            consoleParameter1.Parameter = str2;
            consoleParameter1.Value = "";
            ConsoleParameter consoleParameter4 = consoleParameter1;
            consoleParameterList.Add(consoleParameter4);
          }
        }
        else
        {
          List<ConsoleParameter> consoleParameterList = parameters;
          consoleParameter1 = new ConsoleParameter();
          consoleParameter1.Parameter = "";
          consoleParameter1.Value = str2.Trim(' ', '"');
          ConsoleParameter consoleParameter5 = consoleParameter1;
          consoleParameterList.Add(consoleParameter5);
        }
      }
      return parameters;
    }
  }
}

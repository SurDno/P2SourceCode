using System;
using System.Diagnostics;
using Cofe.Serializations.Converters;
using Engine.Source.Settings;

namespace Engine.Source.Utility
{
  public static class SystemInfoUtility
  {
    private const string numberOfCoresTag = "NumberOfCores";
    private const string numberOfLogicalProcessorsTag = "NumberOfLogicalProcessors";
    private static int numberOfCores = -1;
    private static int numberOfLogicalProcessors = -1;

    public static int NumberOfCores
    {
      get
      {
        if (numberOfCores == -1)
          UpdateData();
        return numberOfCores;
      }
    }

    public static int NumberOfLogicalProcessors
    {
      get
      {
        if (numberOfLogicalProcessors == -1)
          UpdateData();
        return numberOfLogicalProcessors;
      }
    }

    private static void UpdateData()
    {
      numberOfCores = PlayerSettings.Instance.GetInt("NumberOfCores", -1);
      numberOfLogicalProcessors = PlayerSettings.Instance.GetInt("NumberOfLogicalProcessors", -1);
      if (numberOfCores == -1 || numberOfLogicalProcessors == -1)
      {
        try
        {
          UnityEngine.Debug.Log((object) "Try get system info");
          Process process = new Process();
          process.StartInfo.FileName = "wmic.exe";
          process.StartInfo.Arguments = "CPU Get NumberOfCores,NumberOfLogicalProcessors /Format:List";
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.RedirectStandardError = true;
          process.ErrorDataReceived += (a, b) => { };
          process.Start();
          process.BeginErrorReadLine();
          string end = process.StandardOutput.ReadToEnd();
          process.WaitForExit();
          UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Compute result : ").Append(end));
          string str1 = end;
          char[] separator = new char[2]{ '\n', '\r' };
          foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          {
            int length = str2.IndexOf('=');
            int result;
            if (length != -1 && DefaultConverter.TryParseInt(str2.Substring(length + 1), out result))
            {
              switch (str2.Substring(0, length))
              {
                case "NumberOfCores":
                  numberOfCores = result;
                  break;
                case "NumberOfLogicalProcessors":
                  numberOfLogicalProcessors = result;
                  break;
              }
            }
          }
        }
        catch (Exception ex)
        {
          UnityEngine.Debug.LogException(ex);
        }
        if (numberOfCores == -1 || numberOfLogicalProcessors == -1)
        {
          UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Wrong get system info , cores : ").Append(numberOfCores).Append(" , logics : ").Append(numberOfLogicalProcessors));
          numberOfCores = Environment.ProcessorCount;
          numberOfLogicalProcessors = Environment.ProcessorCount;
        }
      }
      else
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Loaded system info , cores : ").Append(numberOfCores).Append(" , logics : ").Append(numberOfLogicalProcessors));
      PlayerSettings.Instance.SetInt("NumberOfCores", numberOfCores);
      PlayerSettings.Instance.SetInt("NumberOfLogicalProcessors", numberOfLogicalProcessors);
    }
  }
}

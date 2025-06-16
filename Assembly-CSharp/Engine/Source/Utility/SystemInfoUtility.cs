// Decompiled with JetBrains decompiler
// Type: Engine.Source.Utility.SystemInfoUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Converters;
using Engine.Source.Settings;
using System;
using System.Diagnostics;

#nullable disable
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
        if (SystemInfoUtility.numberOfCores == -1)
          SystemInfoUtility.UpdateData();
        return SystemInfoUtility.numberOfCores;
      }
    }

    public static int NumberOfLogicalProcessors
    {
      get
      {
        if (SystemInfoUtility.numberOfLogicalProcessors == -1)
          SystemInfoUtility.UpdateData();
        return SystemInfoUtility.numberOfLogicalProcessors;
      }
    }

    private static void UpdateData()
    {
      SystemInfoUtility.numberOfCores = PlayerSettings.Instance.GetInt("NumberOfCores", -1);
      SystemInfoUtility.numberOfLogicalProcessors = PlayerSettings.Instance.GetInt("NumberOfLogicalProcessors", -1);
      if (SystemInfoUtility.numberOfCores == -1 || SystemInfoUtility.numberOfLogicalProcessors == -1)
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
          process.ErrorDataReceived += (DataReceivedEventHandler) ((a, b) => { });
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
                  SystemInfoUtility.numberOfCores = result;
                  break;
                case "NumberOfLogicalProcessors":
                  SystemInfoUtility.numberOfLogicalProcessors = result;
                  break;
              }
            }
          }
        }
        catch (Exception ex)
        {
          UnityEngine.Debug.LogException(ex);
        }
        if (SystemInfoUtility.numberOfCores == -1 || SystemInfoUtility.numberOfLogicalProcessors == -1)
        {
          UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Wrong get system info , cores : ").Append(SystemInfoUtility.numberOfCores).Append(" , logics : ").Append(SystemInfoUtility.numberOfLogicalProcessors));
          SystemInfoUtility.numberOfCores = Environment.ProcessorCount;
          SystemInfoUtility.numberOfLogicalProcessors = Environment.ProcessorCount;
        }
      }
      else
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Loaded system info , cores : ").Append(SystemInfoUtility.numberOfCores).Append(" , logics : ").Append(SystemInfoUtility.numberOfLogicalProcessors));
      PlayerSettings.Instance.SetInt("NumberOfCores", SystemInfoUtility.numberOfCores);
      PlayerSettings.Instance.SetInt("NumberOfLogicalProcessors", SystemInfoUtility.numberOfLogicalProcessors);
    }
  }
}

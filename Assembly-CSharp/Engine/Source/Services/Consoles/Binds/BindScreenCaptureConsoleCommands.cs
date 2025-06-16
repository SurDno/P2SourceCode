// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindScreenCaptureConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Serializations.Converters;
using System;
using System.Globalization;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindScreenCaptureConsoleCommands
  {
    [ConsoleCommand("screen_capture")]
    private static string ScreenCaptureCommand(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 1 && parameters[0].Value == "?")
        return command + " superSize";
      int superSize = 0;
      if (parameters.Length == 0)
      {
        superSize = 1;
      }
      else
      {
        if (parameters.Length != 1)
          return "Error parameter count";
        superSize = DefaultConverter.ParseInt(parameters[0].Parameter);
      }
      if (superSize <= 0)
        return "Error superSize";
      SRDebug.Instance.HideDebugPanel();
      string fileName = DateTime.Now.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ".png";
      fileName = fileName.Replace("/", "_").Replace(":", "_");
      CoroutineService.Instance.WaitFrame((Action) (() => ScreenCapture.CaptureScreenshot(fileName, superSize)));
      return command + " " + fileName;
    }
  }
}

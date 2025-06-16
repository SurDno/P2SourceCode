// Decompiled with JetBrains decompiler
// Type: RootMotion.Warning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion
{
  public static class Warning
  {
    public static bool logged;

    public static void Log(string message, Warning.Logger logger, bool logInEditMode = false)
    {
      if (!logInEditMode && !Application.isPlaying || Warning.logged)
        return;
      if (logger != null)
        logger(message);
      Warning.logged = true;
    }

    public static void Log(string message, Transform context, bool logInEditMode = false)
    {
      if (!logInEditMode && !Application.isPlaying || Warning.logged)
        return;
      Debug.LogWarning((object) message, (Object) context);
      Warning.logged = true;
    }

    public delegate void Logger(string message);
  }
}

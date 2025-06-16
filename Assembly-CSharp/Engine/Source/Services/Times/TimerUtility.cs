// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Times.TimerUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Engine.Source.Services.Times
{
  public static class TimerUtility
  {
    public static string ToLongTimeString(this TimeSpan time)
    {
      return string.Format("{0:d}:{1:d2}:{2:d2}:{3:d2}", (object) time.Days, (object) time.Hours, (object) time.Minutes, (object) time.Seconds);
    }

    public static string ToShortTimeString(this TimeSpan time)
    {
      return string.Format("{0:d2}:{1:d2}", (object) time.Hours, (object) time.Minutes);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.SettingsViewService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Settings
{
  [RuntimeService(new Type[] {typeof (SettingsViewService)})]
  public class SettingsViewService
  {
    [Inspected]
    private static List<object> settings = new List<object>();

    public static void AddSettings(object setting)
    {
      SettingsViewService.settings.Add(setting);
      SettingsViewService.settings.Sort((Comparison<object>) ((a, b) => TypeUtility.GetTypeName(a.GetType()).CompareTo(TypeUtility.GetTypeName(b.GetType()))));
    }
  }
}

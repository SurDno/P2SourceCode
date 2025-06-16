using Cofe.Utility;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;

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

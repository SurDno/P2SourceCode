// Decompiled with JetBrains decompiler
// Type: UnityEditor.EditorGUILayout2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace UnityEditor
{
  public static class EditorGUILayout2
  {
    private const string nameNothing = "Nothing";
    private const string nameEverything = "Everything";

    public static string GetEnumValueName(Enum enumValue)
    {
      int[] values = (int[]) Enum.GetValues(enumValue.GetType());
      string[] names = Enum.GetNames(enumValue.GetType());
      string enumValueName = "";
      int num = (int) enumValue;
      for (int index = 0; index < values.Length; ++index)
      {
        if ((values[index] & num) != 0)
        {
          if (enumValueName != "")
            enumValueName += ", ";
          enumValueName += names[index];
        }
      }
      if (enumValueName == "")
        enumValueName = "Nothing";
      return enumValueName;
    }
  }
}

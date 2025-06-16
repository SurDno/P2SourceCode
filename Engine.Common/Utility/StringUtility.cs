// Decompiled with JetBrains decompiler
// Type: Engine.Common.Utility.StringUtility
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

#nullable disable
namespace Engine.Common.Utility
{
  public static class StringUtility
  {
    public static bool NextSubstring(
      string value,
      string separator,
      ref int index,
      ref string result)
    {
      if (index == -1)
        return false;
      int num = value.IndexOf(separator, index);
      if (num == -1)
      {
        result = value.Substring(index);
        index = -1;
        return true;
      }
      result = value.Substring(index, num - index);
      index = num + separator.Length;
      return true;
    }
  }
}

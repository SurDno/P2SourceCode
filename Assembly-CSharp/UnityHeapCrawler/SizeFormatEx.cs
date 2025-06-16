using JetBrains.Annotations;
using System;
using System.Globalization;

namespace UnityHeapCrawler
{
  public static class SizeFormatEx
  {
    [NotNull]
    public static string Format(this SizeFormat format, long size)
    {
      if (format == SizeFormat.Precise)
        return size.ToString();
      long num1 = 1;
      int num2;
      for (num2 = 0; num1 * 1024L < size && num2 < 4; ++num2)
        num1 *= 1024L;
      double num3 = 1.0 * (double) size / (double) num1;
      string str1 = (num2 != 0 ? (num3 < 9.995 ? num3.ToString("F2", (IFormatProvider) CultureInfo.InvariantCulture) : num3.ToString("F1", (IFormatProvider) CultureInfo.InvariantCulture)) : size.ToString((IFormatProvider) CultureInfo.InvariantCulture)) + " ";
      string str2;
      switch (num2)
      {
        case 0:
          str2 = str1 + "b";
          break;
        case 1:
          str2 = str1 + "kb";
          break;
        case 2:
          str2 = str1 + "mb";
          break;
        case 3:
          str2 = str1 + "gb";
          break;
        case 4:
          str2 = str1 + "tb";
          break;
        default:
          str2 = str1 + "Unknown Qualifier";
          break;
      }
      if (format == SizeFormat.Short)
        return str2;
      return str2 + " (" + (object) size + ")";
    }
  }
}

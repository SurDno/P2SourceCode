using System.IO;
using System.Threading;

public static class SRFileUtil
{
  public static void DeleteDirectory(string path)
  {
    try
    {
      Directory.Delete(path, true);
    }
    catch (IOException ex)
    {
      Thread.Sleep(0);
      Directory.Delete(path, true);
    }
  }

  public static string GetBytesReadable(long i)
  {
    string str1 = i < 0L ? "-" : "";
    double num1 = i < 0L ? (double) -i : (double) i;
    string str2;
    double num2;
    if (i >= 1152921504606846976L)
    {
      str2 = "EB";
      num2 = (double) (i >> 50);
    }
    else if (i >= 1125899906842624L)
    {
      str2 = "PB";
      num2 = (double) (i >> 40);
    }
    else if (i >= 1099511627776L)
    {
      str2 = "TB";
      num2 = (double) (i >> 30);
    }
    else if (i >= 1073741824L)
    {
      str2 = "GB";
      num2 = (double) (i >> 20);
    }
    else if (i >= 1048576L)
    {
      str2 = "MB";
      num2 = (double) (i >> 10);
    }
    else
    {
      if (i < 1024L)
        return i.ToString(str1 + "0 B");
      str2 = "KB";
      num2 = (double) i;
    }
    double num3 = num2 / 1024.0;
    return str1 + num3.ToString("0.### ") + str2;
  }
}

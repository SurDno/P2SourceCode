// Decompiled with JetBrains decompiler
// Type: Scripts.Utility.CompressUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.IO;
using System.IO.Compression;

#nullable disable
namespace Scripts.Utility
{
  public static class CompressUtility
  {
    public static byte[] CompressData(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Compress))
          gzipStream.Write(data, 0, data.Length);
        return memoryStream.ToArray();
      }
    }

    public static byte[] DecompressData(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream(data))
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Decompress))
        {
          using (MemoryStream destination = new MemoryStream())
          {
            gzipStream.CopyTo((Stream) destination);
            return destination.ToArray();
          }
        }
      }
    }
  }
}

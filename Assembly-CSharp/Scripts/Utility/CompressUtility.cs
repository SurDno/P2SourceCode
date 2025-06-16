using System.IO;
using System.IO.Compression;

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

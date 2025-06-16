using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using Cofe.Serializations.Data.Xml;
using Engine.Common;
using Engine.Common.Commons.Converters;

namespace AssetDatabases
{
  public static class AssetDatabaseUtility
  {
    public static bool IgnoreAsset(string path)
    {
      if (!IsContentFolder(path))
        return true;
      foreach (string complexExt in Paths.ComplexExts)
      {
        if (path.EndsWith(complexExt))
          return false;
      }
      string lowerInvariant = Path.GetExtension(path).ToLowerInvariant();
      return !Paths.ReferenceExts.Contains(lowerInvariant) && !Paths.AudioExts.Contains(lowerInvariant);
    }

    private static bool IsContentFolder(string path)
    {
      foreach (string contentFolder in Paths.ContentFolders)
      {
        if (path.StartsWith(contentFolder))
          return true;
      }
      return false;
    }

    public static string ConvertToResourcePath(string path)
    {
      string str = "/Resources/";
      int num = path.LastIndexOf(str);
      if (num == -1)
        return "";
      path = path.Substring(num + str.Length);
      int length = path.LastIndexOf(".");
      if (length != -1)
        path = path.Substring(0, length);
      return path;
    }

    public static string GetFileName(string path)
    {
      int num = path.LastIndexOf('/');
      if (num != -1)
        path = path.Substring(num + 1);
      int length = path.LastIndexOf('.');
      if (length != -1)
        path = path.Substring(0, length);
      return path;
    }

    public static T LoadFromFile<T>(string fileName) where T : class
    {
      try
      {
        using (FileStream fileStream = File.OpenRead(fileName + ".gz"))
        {
          using (GZipStream inStream = new GZipStream(fileStream, CompressionMode.Decompress))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(inStream);
            return DefaultDataReadUtility.ReadSerialize<T>(new XmlNodeDataReader(xmlDocument.DocumentElement, fileName));
          }
        }
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ("Error open file : " + fileName + ".gz , error : " + ex));
        return default (T);
      }
    }
  }
}

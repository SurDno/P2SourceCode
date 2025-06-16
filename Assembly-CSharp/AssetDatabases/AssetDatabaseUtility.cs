// Decompiled with JetBrains decompiler
// Type: AssetDatabases.AssetDatabaseUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Engine.Common;
using Engine.Common.Commons.Converters;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using UnityEngine;

#nullable disable
namespace AssetDatabases
{
  public static class AssetDatabaseUtility
  {
    public static bool IgnoreAsset(string path)
    {
      if (!AssetDatabaseUtility.IsContentFolder(path))
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
          using (GZipStream inStream = new GZipStream((Stream) fileStream, CompressionMode.Decompress))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load((Stream) inStream);
            return DefaultDataReadUtility.ReadSerialize<T>((IDataReader) new XmlNodeDataReader((XmlNode) xmlDocument.DocumentElement, fileName));
          }
        }
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ("Error open file : " + fileName + ".gz , error : " + (object) ex));
        return default (T);
      }
    }
  }
}

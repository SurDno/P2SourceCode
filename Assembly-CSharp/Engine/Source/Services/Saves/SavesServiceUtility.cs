// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Saves.SavesServiceUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Engine.Source.Saves;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.Saves
{
  public static class SavesServiceUtility
  {
    public static XmlDocument LoadDocument(string fileName)
    {
      if (File.Exists(fileName + ".gz"))
      {
        try
        {
          using (FileStream fileStream = File.OpenRead(fileName + ".gz"))
          {
            using (GZipStream inStream = new GZipStream((Stream) fileStream, CompressionMode.Decompress))
            {
              XmlDocument xmlDocument = new XmlDocument();
              xmlDocument.Load((Stream) inStream);
              return xmlDocument;
            }
          }
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ("Error open file : " + fileName + ".gz , error : " + (object) ex));
          return (XmlDocument) null;
        }
      }
      else if (File.Exists(fileName))
      {
        try
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load(fileName);
          return xmlDocument;
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ("Error open file : " + fileName + " , error : " + (object) ex));
          return (XmlDocument) null;
        }
      }
      else
      {
        Debug.LogError((object) ("File not found : " + fileName));
        return (XmlDocument) null;
      }
    }

    public static void SaveToFile(
      string nodeName,
      ISavesController serializable,
      string fileName,
      bool compress)
    {
      if (compress)
      {
        using (FileStream fileStream = File.Create(fileName + ".gz"))
        {
          using (GZipStream gzipStream = new GZipStream((Stream) fileStream, CompressionMode.Compress))
          {
            using (StreamWriter stream = new StreamWriter((Stream) gzipStream, Encoding.UTF8))
            {
              StreamDataWriter element = new StreamDataWriter(stream);
              element.Begin(nodeName, (System.Type) null, true);
              serializable.Save((IDataWriter) element, fileName);
              element.End(nodeName, true);
            }
          }
        }
      }
      else
      {
        using (StreamWriter stream = new StreamWriter(fileName, false, Encoding.UTF8))
        {
          StreamDataWriter element = new StreamDataWriter(stream);
          element.Begin(nodeName, (System.Type) null, true);
          serializable.Save((IDataWriter) element, fileName);
          element.End(nodeName, true);
        }
      }
    }
  }
}

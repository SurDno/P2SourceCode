using System;
using System.IO;
using System.Text;
using System.Xml;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;

public static class SerializeUtility
{
  public const string RootNodeName = "Object";

  public static void Serialize<T>(string path, T template) where T : class
  {
    if (template == null)
    {
      Debug.LogError((object) ("Template is null , type : " + TypeUtility.GetTypeName(typeof (T))));
    }
    else
    {
      try
      {
        using (StreamWriter stream = new StreamWriter(path, false, Encoding.UTF8))
        {
          StreamDataWriter writer = new StreamDataWriter(stream);
          if (template is ISerializeDataWrite serializeDataWrite)
          {
            Type type = ProxyFactory.GetType(template.GetType());
            writer.Begin("Object", type, true);
            serializeDataWrite.DataWrite(writer);
            writer.End("Object", true);
          }
          else
            Debug.LogError((object) ("Type : " + TypeUtility.GetTypeName(template.GetType()) + " is not " + typeof (ISerializeDataWrite).Name));
        }
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }
  }

  public static T Deserialize<T>(string path) where T : class
  {
    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
      return Deserialize<T>(fileStream, path);
  }

  public static T Deserialize<T>(Stream stream, string context) where T : class
  {
    if (stream.Length <= 0L)
      return default (T);
    XmlDocument node = new XmlDocument();
    try
    {
      node.Load(stream);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) (ex + " : " + context));
      return default (T);
    }
    T obj = DefaultDataReadUtility.ReadSerialize<T>(new XmlNodeDataReader(node, context), "Object");
    if (obj is IFactoryProduct factoryProduct)
      factoryProduct.ConstructComplete();
    return obj;
  }

  public static T Deserialize<T>(XmlNode node, string context) where T : class
  {
    T obj = DefaultDataReadUtility.ReadSerialize<T>(new XmlNodeDataReader(node, context));
    if (obj is IFactoryProduct factoryProduct)
      factoryProduct.ConstructComplete();
    return obj;
  }
}

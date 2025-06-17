using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Cofe.Utility;
using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using VirtualMachine.Common.Data;

namespace VirtualMachine.Data
{
  public static class EditorDataReadUtility
  {
    public static T ReadReference<T>(XmlReader xml, IDataCreator creator) where T : class
    {
      ulong id = DefaultConverter.ParseUlong(XmlReaderUtility.ReadContent(xml));
      if (id == 0UL)
        return default (T);
      IObject objectThreadSave = creator.GetOrCreateObjectThreadSave(id);
      if (objectThreadSave == null)
      {
        Logger.AddError("Object id : " + id + " not found");
        return default (T);
      }
      if (objectThreadSave is T obj)
        return obj;
      Logger.AddError("Object id : " + id + " is not type : " + TypeUtility.GetTypeName(typeof (T)));
      return default (T);
    }

    public static List<T> ReadReferenceList<T>(XmlReader xml, IDataCreator creator, List<T> source) where T : class
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      if (source == null)
      {
        source = new List<T>(capacity);
      }
      else
      {
        source.Clear();
        source.Capacity = capacity;
      }
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            T obj = ReadReference<T>(xml, creator);
            source.Add(obj);
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static List<T> ReadEnumList<T>(XmlReader xml, List<T> source) where T : struct, IComparable, IConvertible, IFormattable
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      if (source == null)
      {
        source = new List<T>(capacity);
      }
      else
      {
        source.Clear();
        source.Capacity = capacity;
      }
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            DefaultConverter.TryParseEnum(XmlReaderUtility.ReadContent(xml), out T result);
            source.Add(result);
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static List<string> ReadValueList(XmlReader xml, List<string> source)
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      if (source == null)
      {
        source = new List<string>(capacity);
      }
      else
      {
        source.Clear();
        source.Capacity = capacity;
      }
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            string str = XmlReaderUtility.ReadContent(xml);
            source.Add(str);
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static List<Guid> ReadValueList(XmlReader xml, List<Guid> source)
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      if (source == null)
      {
        source = new List<Guid>(capacity);
      }
      else
      {
        source.Clear();
        source.Capacity = capacity;
      }
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            string str = XmlReaderUtility.ReadContent(xml);
            source.Add(DefaultConverter.ParseGuid(str));
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static List<ulong> ReadValueList(XmlReader xml, List<ulong> source)
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      if (source == null)
      {
        source = new List<ulong>(capacity);
      }
      else
      {
        source.Clear();
        source.Capacity = capacity;
      }
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            string str = XmlReaderUtility.ReadContent(xml);
            source.Add(DefaultConverter.ParseUlong(str));
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static List<T> ReadEditorDataSerializableList<T>(
      XmlReader xml,
      IDataCreator creator,
      List<T> source)
      where T : class, IEditorDataReader, new()
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      if (source == null)
      {
        source = new List<T>(capacity);
      }
      else
      {
        source.Clear();
        source.Capacity = capacity;
      }
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            T obj = ReadEditorDataSerializable<T>(xml, creator, "");
            source.Add(obj);
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static List<T> ReadSerializableList<T>(XmlReader xml, List<T> source) where T : class, IVMStringSerializable, new()
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      if (source == null)
      {
        source = new List<T>(capacity);
      }
      else
      {
        source.Clear();
        source.Capacity = capacity;
      }
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            T obj = ReadSerializable<T>(xml);
            source.Add(obj);
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static Dictionary<ulong, T> ReadUlongEditorDataSerializableDictionary<T>(
      XmlReader xml,
      IDataCreator creator,
      Dictionary<ulong, T> source)
      where T : class, IEditorDataReader, new()
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      source = new Dictionary<ulong, T>(capacity, UlongComparer.Instance);
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            xml.MoveToAttribute("key");
            ulong key = DefaultConverter.ParseUlong(xml.Value);
            T obj = ReadEditorDataSerializable<T>(xml, creator, "");
            source.Add(key, obj);
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static Dictionary<string, T> ReadStringReferenceDictionary<T>(
      XmlReader xml,
      IDataCreator creator,
      Dictionary<string, T> source)
      where T : class
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      source = new Dictionary<string, T>(capacity);
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Element)
          {
            xml.MoveToAttribute("key");
            string key = xml.Value;
            T obj = ReadReference<T>(xml, creator);
            source.Add(key, obj);
          }
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return source;
    }

    public static string ReadValue(XmlReader xml, string tmp) => XmlReaderUtility.ReadContent(xml);

    public static int ReadValue(XmlReader xml, int tmp)
    {
      return DefaultConverter.ParseInt(XmlReaderUtility.ReadContent(xml));
    }

    public static long ReadValue(XmlReader xml, long tmp)
    {
      return DefaultConverter.ParseLong(XmlReaderUtility.ReadContent(xml));
    }

    public static bool ReadValue(XmlReader xml, bool tmp)
    {
      return DefaultConverter.ParseBool(XmlReaderUtility.ReadContent(xml));
    }

    public static ulong ReadValue(XmlReader xml, ulong tmp)
    {
      return DefaultConverter.ParseUlong(XmlReaderUtility.ReadContent(xml));
    }

    public static float ReadValue(XmlReader xml, float tmp)
    {
      return DefaultConverter.ParseFloat(XmlReaderUtility.ReadContent(xml));
    }

    public static Guid ReadValue(XmlReader xml, Guid tmp)
    {
      return DefaultConverter.ParseGuid(XmlReaderUtility.ReadContent(xml));
    }

    public static HierarchyGuid ReadValue(XmlReader xml, HierarchyGuid tmp)
    {
      return new HierarchyGuid(XmlReaderUtility.ReadContent(xml));
    }

    public static T ReadEditorDataSerializable<T>(
      XmlReader xml,
      IDataCreator creator,
      string typeContext)
      where T : class, IEditorDataReader, new()
    {
      T obj = new T();
      obj.EditorDataRead(xml, creator, typeContext);
      return obj;
    }

    public static VMType ReadTypeSerializable(XmlReader xml)
    {
      return VMTypePool.GetType(XmlReaderUtility.ReadContent(xml));
    }

    public static T ReadSerializable<T>(XmlReader xml) where T : class, IVMStringSerializable, new()
    {
      string data = XmlReaderUtility.ReadContent(xml);
      T obj = new T();
      obj.Read(data);
      return obj;
    }

    public static T ReadEnum<T>(XmlReader xml) where T : struct, IComparable, IConvertible, IFormattable
    {
      DefaultConverter.TryParseEnum(XmlReaderUtility.ReadContent(xml), out T result);
      return result;
    }

    public static object ReadObjectValue(XmlReader xml)
    {
      return XmlReaderUtility.ReadContent(xml);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Data.SaveLoad
{
  public static class VMSaveLoadManager
  {
    public static void LoadDynamicSerializable(XmlElement node, IDynamicLoadSerializable value)
    {
      value.LoadFromXML(node);
    }

    public static void LoadList(XmlElement listNode, List<string> list)
    {
      if (list == null)
        list = new List<string>();
      else
        list.Clear();
      foreach (XmlNode childNode in listNode.ChildNodes)
        list.Add(childNode.InnerText);
    }

    public static void LoadList(XmlElement listNode, List<int> list)
    {
      if (list == null)
        list = new List<int>();
      else
        list.Clear();
      foreach (XmlNode childNode in listNode.ChildNodes)
        list.Add(DefaultConverter.ParseInt(childNode.InnerText));
    }

    public static void LoadList(XmlElement listNode, List<ulong> list)
    {
      if (list == null)
        list = new List<ulong>();
      else
        list.Clear();
      foreach (XmlNode childNode in listNode.ChildNodes)
        list.Add(DefaultConverter.ParseUlong(childNode.InnerText));
    }

    public static void LoadDynamiSerializableList<T>(XmlElement listNode, List<T> list) where T : IDynamicLoadSerializable, new()
    {
      if (listNode == null)
        return;
      if (list == null)
        list = new List<T>();
      else
        list.Clear();
      foreach (XmlNode childNode in listNode.ChildNodes)
      {
        T obj = new T();
        obj.LoadFromXML((XmlElement) childNode);
        list.Add(obj);
      }
    }

    public static Guid ReadGuid(XmlNode valueNode)
    {
      return DefaultConverter.ParseGuid(valueNode.InnerText);
    }

    public static float ReadFloat(XmlNode valueNode)
    {
      return DefaultConverter.ParseFloat(valueNode.InnerText);
    }

    public static int ReadInt(XmlNode valueNode) => DefaultConverter.ParseInt(valueNode.InnerText);

    public static bool ReadBool(XmlNode valueNode)
    {
      return DefaultConverter.ParseBool(valueNode.InnerText);
    }

    public static ulong ReadUlong(XmlNode valueNode)
    {
      return DefaultConverter.ParseUlong(valueNode.InnerText);
    }

    public static T ReadEnum<T>(XmlNode valueNode) where T : struct, IComparable, IFormattable, IConvertible
    {
      T result;
      DefaultConverter.TryParseEnum(valueNode.InnerText, out result);
      return result;
    }

    public static T ReadValue<T>(XmlNode valueNode)
    {
      return (T) ReadValue(valueNode, typeof (T));
    }

    public static object ReadValue(XmlNode valueNode, Type objType)
    {
      if (valueNode == null)
      {
        Logger.AddError("Saveload error: attempt to read null node!!!");
        return Activator.CreateInstance(objType);
      }
      if (!typeof (IDynamicLoadSerializable).IsAssignableFrom(objType))
        return StringSerializer.ReadValue(valueNode.InnerText, objType);
      Type type = BaseSerializer.GetRealRefType(objType);
      if (type == null)
        type = objType;
      IDynamicLoadSerializable instance = (IDynamicLoadSerializable) Activator.CreateInstance(type);
      instance.LoadFromXML((XmlElement) valueNode);
      return instance;
    }
  }
}

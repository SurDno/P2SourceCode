using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Source.Connections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Tools.Serializations.Converters
{
  public static class UnityDataReadUtility
  {
    public static Vector2 Read(IDataReader reader, string name, Vector2 value)
    {
      return UnityConverter.ParseVector2(reader.ReadSimple(name));
    }

    public static Vector3 Read(IDataReader reader, string name, Vector3 value)
    {
      return UnityConverter.ParseVector3(reader.ReadSimple(name));
    }

    public static Vector4 Read(IDataReader reader, string name, Vector4 value)
    {
      return UnityConverter.ParseVector4(reader.ReadSimple(name));
    }

    public static Quaternion Read(IDataReader reader, string name, Quaternion value)
    {
      return UnityConverter.ParseQuaternion(reader.ReadSimple(name));
    }

    public static Color Read(IDataReader reader, string name, Color value)
    {
      return UnityConverter.ParseColor(reader.ReadSimple(name));
    }

    public static LayerMask Read(IDataReader reader, string name, LayerMask value)
    {
      return new LayerMask()
      {
        value = DefaultConverter.ParseInt(reader.ReadSimple(name))
      };
    }

    public static GradientAlphaKey ReadGradientAlphaKey(IDataReader reader)
    {
      return new GradientAlphaKey(DefaultConverter.ParseFloat(reader.ReadSimple("Alpha")), DefaultConverter.ParseFloat(reader.ReadSimple("Time")));
    }

    public static GradientColorKey ReadGradientColorKey(IDataReader reader)
    {
      return new GradientColorKey(new Color(DefaultConverter.ParseFloat(reader.ReadSimple("R")), DefaultConverter.ParseFloat(reader.ReadSimple("G")), DefaultConverter.ParseFloat(reader.ReadSimple("B")), DefaultConverter.ParseFloat(reader.ReadSimple("A"))), DefaultConverter.ParseFloat(reader.ReadSimple("Time")));
    }

    public static Typed<T> Read<T>(IDataReader reader, string name, Typed<T> value) where T : class, IObject
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? new Typed<T>() : new Typed<T>(DefaultDataReadUtility.Read(child, "Id", value.Id));
    }

    public static UnitySubAsset<T> Read<T>(IDataReader reader, string name, UnitySubAsset<T> value) where T : UnityEngine.Object
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? new UnitySubAsset<T>() : new UnitySubAsset<T>(DefaultDataReadUtility.Read(child, "Id", value.Id), DefaultDataReadUtility.Read(child, "Name", value.Name));
    }

    public static UnityAsset<T> Read<T>(IDataReader reader, string name, UnityAsset<T> value) where T : UnityEngine.Object
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? new UnityAsset<T>() : new UnityAsset<T>(DefaultDataReadUtility.Read(child, "Id", value.Id));
    }

    public static List<GradientAlphaKey> ReadList(
      IDataReader reader,
      string name,
      List<GradientAlphaKey> value)
    {
      value.Clear();
      foreach (IDataReader child in reader.GetChild(name).GetChilds())
      {
        GradientAlphaKey gradientAlphaKey = UnityDataReadUtility.ReadGradientAlphaKey(child);
        value.Add(gradientAlphaKey);
      }
      return value;
    }

    public static List<GradientColorKey> ReadList(
      IDataReader reader,
      string name,
      List<GradientColorKey> value)
    {
      value.Clear();
      foreach (IDataReader child in reader.GetChild(name).GetChilds())
      {
        GradientColorKey gradientColorKey = UnityDataReadUtility.ReadGradientColorKey(child);
        value.Add(gradientColorKey);
      }
      return value;
    }

    public static List<Typed<T>> ReadList<T>(IDataReader reader, string name, List<Typed<T>> value) where T : class, IObject
    {
      if (value == null)
        value = new List<Typed<T>>();
      else
        value.Clear();
      foreach (IDataReader child in reader.GetChild(name).GetChilds())
      {
        Guid guid = DefaultConverter.ParseGuid(child.ReadSimple("Id"));
        value.Add(new Typed<T>(guid));
      }
      return value;
    }

    public static List<UnitySubAsset<T>> ReadList<T>(
      IDataReader reader,
      string name,
      List<UnitySubAsset<T>> value)
      where T : UnityEngine.Object
    {
      value.Clear();
      foreach (IDataReader child in reader.GetChild(name).GetChilds())
      {
        Guid guid = DefaultConverter.ParseGuid(child.ReadSimple("Id"));
        string name1 = child.ReadSimple("Name");
        value.Add(new UnitySubAsset<T>(guid, name1));
      }
      return value;
    }

    public static List<UnityAsset<T>> ReadList<T>(
      IDataReader reader,
      string name,
      List<UnityAsset<T>> value)
      where T : UnityEngine.Object
    {
      value.Clear();
      foreach (IDataReader child in reader.GetChild(name).GetChilds())
      {
        Guid guid = DefaultConverter.ParseGuid(child.ReadSimple("Id"));
        value.Add(new UnityAsset<T>(guid));
      }
      return value;
    }
  }
}

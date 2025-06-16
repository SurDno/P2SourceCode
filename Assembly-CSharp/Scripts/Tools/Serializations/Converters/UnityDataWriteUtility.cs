// Decompiled with JetBrains decompiler
// Type: Scripts.Tools.Serializations.Converters.UnityDataWriteUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Source.Connections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Scripts.Tools.Serializations.Converters
{
  public static class UnityDataWriteUtility
  {
    public static void Write(IDataWriter writer, string name, Vector2 value)
    {
      writer.Begin(name, (System.Type) null, false);
      writer.Write(UnityConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, Vector3 value)
    {
      writer.Begin(name, (System.Type) null, false);
      writer.Write(UnityConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, Vector4 value)
    {
      writer.Begin(name, (System.Type) null, false);
      writer.Write(UnityConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, Quaternion value)
    {
      writer.Begin(name, (System.Type) null, false);
      writer.Write(UnityConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, Color value)
    {
      writer.Begin(name, (System.Type) null, false);
      writer.Write(UnityConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, LayerMask value)
    {
      writer.Begin(name, (System.Type) null, false);
      writer.Write(DefaultConverter.ToString(value.value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, GradientAlphaKey value)
    {
      writer.Begin(name, (System.Type) null, true);
      DefaultDataWriteUtility.Write(writer, "Alpha", value.alpha);
      DefaultDataWriteUtility.Write(writer, "Time", value.time);
      writer.End(name, true);
    }

    public static void Write(IDataWriter writer, string name, GradientColorKey value)
    {
      writer.Begin(name, (System.Type) null, true);
      DefaultDataWriteUtility.Write(writer, "R", value.color.r);
      DefaultDataWriteUtility.Write(writer, "G", value.color.g);
      DefaultDataWriteUtility.Write(writer, "B", value.color.b);
      DefaultDataWriteUtility.Write(writer, "A", value.color.a);
      DefaultDataWriteUtility.Write(writer, "Time", value.time);
      writer.End(name, true);
    }

    public static void Write<T>(IDataWriter writer, string name, UnitySubAsset<T> value) where T : UnityEngine.Object
    {
      writer.Begin(name, (System.Type) null, true);
      DefaultDataWriteUtility.Write(writer, "Id", value.Id);
      DefaultDataWriteUtility.Write(writer, "Name", value.Name);
      writer.End(name, true);
    }

    public static void Write<T>(IDataWriter writer, string name, UnityAsset<T> value) where T : UnityEngine.Object
    {
      writer.Begin(name, (System.Type) null, true);
      DefaultDataWriteUtility.Write(writer, "Id", value.Id);
      writer.End(name, true);
    }

    public static void Write<T>(IDataWriter writer, string name, Typed<T> value) where T : class, IObject
    {
      writer.Begin(name, (System.Type) null, true);
      DefaultDataWriteUtility.Write(writer, "Id", value.Id);
      writer.End(name, true);
    }

    public static void WriteList(IDataWriter writer, string name, List<GradientAlphaKey> value)
    {
      writer.Begin(name, (System.Type) null, true);
      foreach (GradientAlphaKey gradientAlphaKey in value)
        UnityDataWriteUtility.Write(writer, "Item", gradientAlphaKey);
      writer.End(name, true);
    }

    public static void WriteList(IDataWriter writer, string name, List<GradientColorKey> value)
    {
      writer.Begin(name, (System.Type) null, true);
      foreach (GradientColorKey gradientColorKey in value)
        UnityDataWriteUtility.Write(writer, "Item", gradientColorKey);
      writer.End(name, true);
    }

    public static void WriteList<T>(IDataWriter writer, string name, List<Typed<T>> value) where T : class, IObject
    {
      System.Type type = typeof (T);
      writer.Begin(name, (System.Type) null, true);
      foreach (Typed<T> typed in value)
        UnityDataWriteUtility.Write<T>(writer, "Item", typed);
      writer.End(name, true);
    }
  }
}

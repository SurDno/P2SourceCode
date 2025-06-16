// Decompiled with JetBrains decompiler
// Type: UnityHeapCrawler.TypeData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

#nullable disable
namespace UnityHeapCrawler
{
  public class TypeData
  {
    private static Dictionary<Type, TypeData> seenTypeData;
    private static Dictionary<Type, TypeData> seenTypeDataNested;

    public int Size { get; private set; }

    public List<FieldInfo> DynamicSizedFields { get; private set; }

    public static void Clear() => TypeData.seenTypeData = (Dictionary<Type, TypeData>) null;

    public static void Start()
    {
      TypeData.seenTypeData = new Dictionary<Type, TypeData>();
      TypeData.seenTypeDataNested = new Dictionary<Type, TypeData>();
    }

    public static TypeData Get(Type type)
    {
      TypeData typeData;
      if (!TypeData.seenTypeData.TryGetValue(type, out typeData))
      {
        typeData = new TypeData(type);
        TypeData.seenTypeData[type] = typeData;
      }
      return typeData;
    }

    public static TypeData GetNested(Type type)
    {
      TypeData nested;
      if (!TypeData.seenTypeDataNested.TryGetValue(type, out nested))
      {
        nested = new TypeData(type, true);
        TypeData.seenTypeDataNested[type] = nested;
      }
      return nested;
    }

    public TypeData(Type type, bool nested = false)
    {
      Type baseType = type.BaseType;
      if (baseType != (Type) null && baseType != typeof (object) && baseType != typeof (ValueType) && baseType != typeof (Array) && baseType != typeof (Enum))
      {
        TypeData nested1 = TypeData.GetNested(baseType);
        this.Size += nested1.Size;
        if (nested1.DynamicSizedFields != null)
          this.DynamicSizedFields = new List<FieldInfo>((IEnumerable<FieldInfo>) nested1.DynamicSizedFields);
      }
      if (type.IsPointer)
        this.Size = IntPtr.Size;
      else if (type.IsArray)
      {
        Type elementType = type.GetElementType();
        this.Size = (elementType.IsValueType || elementType.IsPrimitive || elementType.IsEnum ? 3 : 4) * IntPtr.Size;
      }
      else if (type.IsPrimitive)
        this.Size = Marshal.SizeOf(type);
      else if (type.IsEnum)
      {
        this.Size = Marshal.SizeOf(Enum.GetUnderlyingType(type));
      }
      else
      {
        if (!nested && type.IsClass)
          this.Size = 2 * IntPtr.Size;
        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
          this.ProcessField(field, field.FieldType);
        if (!nested && type.IsClass)
        {
          this.Size = Math.Max(3 * IntPtr.Size, this.Size);
          int num = this.Size % IntPtr.Size;
          if (num != 0)
            this.Size += IntPtr.Size - num;
        }
      }
    }

    private void ProcessField(FieldInfo field, Type fieldType)
    {
      if (TypeData.IsStaticallySized(fieldType))
      {
        this.Size += TypeData.GetStaticSize(fieldType);
      }
      else
      {
        if (!fieldType.IsValueType && !fieldType.IsPrimitive && !fieldType.IsEnum)
          this.Size += IntPtr.Size;
        if (fieldType.IsPointer)
          return;
        if (this.DynamicSizedFields == null)
          this.DynamicSizedFields = new List<FieldInfo>();
        this.DynamicSizedFields.Add(field);
      }
    }

    private static bool IsStaticallySized(Type type)
    {
      if (type.IsPointer || type.IsArray || type.IsClass || type.IsInterface)
        return false;
      if (type.IsPrimitive || type.IsEnum)
        return true;
      foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (!TypeData.IsStaticallySized(field.FieldType))
          return false;
      }
      return true;
    }

    private static int GetStaticSize(Type type)
    {
      if (type.IsPrimitive)
        return Marshal.SizeOf(type);
      if (type.IsEnum)
        return Marshal.SizeOf(Enum.GetUnderlyingType(type));
      int staticSize = 0;
      foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        staticSize += TypeData.GetStaticSize(field.FieldType);
      return staticSize;
    }
  }
}

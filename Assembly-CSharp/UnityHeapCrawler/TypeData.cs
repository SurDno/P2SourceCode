﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UnityHeapCrawler
{
  public class TypeData
  {
    private static Dictionary<Type, TypeData> seenTypeData;
    private static Dictionary<Type, TypeData> seenTypeDataNested;

    public int Size { get; private set; }

    public List<FieldInfo> DynamicSizedFields { get; private set; }

    public static void Clear() => seenTypeData = null;

    public static void Start()
    {
      seenTypeData = new Dictionary<Type, TypeData>();
      seenTypeDataNested = new Dictionary<Type, TypeData>();
    }

    public static TypeData Get(Type type)
    {
      if (!seenTypeData.TryGetValue(type, out TypeData typeData))
      {
        typeData = new TypeData(type);
        seenTypeData[type] = typeData;
      }
      return typeData;
    }

    public static TypeData GetNested(Type type)
    {
      if (!seenTypeDataNested.TryGetValue(type, out TypeData nested))
      {
        nested = new TypeData(type, true);
        seenTypeDataNested[type] = nested;
      }
      return nested;
    }

    public TypeData(Type type, bool nested = false)
    {
      Type baseType = type.BaseType;
      if (baseType != null && baseType != typeof (object) && baseType != typeof (ValueType) && baseType != typeof (Array) && baseType != typeof (Enum))
      {
        TypeData nested1 = GetNested(baseType);
        Size += nested1.Size;
        if (nested1.DynamicSizedFields != null)
          DynamicSizedFields = [..nested1.DynamicSizedFields];
      }
      if (type.IsPointer)
        Size = IntPtr.Size;
      else if (type.IsArray)
      {
        Type elementType = type.GetElementType();
        Size = (elementType.IsValueType || elementType.IsPrimitive || elementType.IsEnum ? 3 : 4) * IntPtr.Size;
      }
      else if (type.IsPrimitive)
        Size = Marshal.SizeOf(type);
      else if (type.IsEnum)
      {
        Size = Marshal.SizeOf(Enum.GetUnderlyingType(type));
      }
      else
      {
        if (!nested && type.IsClass)
          Size = 2 * IntPtr.Size;
        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
          ProcessField(field, field.FieldType);
        if (!nested && type.IsClass)
        {
          Size = Math.Max(3 * IntPtr.Size, Size);
          int num = Size % IntPtr.Size;
          if (num != 0)
            Size += IntPtr.Size - num;
        }
      }
    }

    private void ProcessField(FieldInfo field, Type fieldType)
    {
      if (IsStaticallySized(fieldType))
      {
        Size += GetStaticSize(fieldType);
      }
      else
      {
        if (!fieldType.IsValueType && !fieldType.IsPrimitive && !fieldType.IsEnum)
          Size += IntPtr.Size;
        if (fieldType.IsPointer)
          return;
        if (DynamicSizedFields == null)
          DynamicSizedFields = [];
        DynamicSizedFields.Add(field);
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
        if (!IsStaticallySized(field.FieldType))
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
        staticSize += GetStaticSize(field.FieldType);
      return staticSize;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Cinemachine.Utility.ReflectionHelpers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

#nullable disable
namespace Cinemachine.Utility
{
  [DocumentationSorting(0.0f, DocumentationSortingAttribute.Level.Undoc)]
  public static class ReflectionHelpers
  {
    public static void CopyFields(object src, object dst, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    {
      if (src == null || dst == null)
        return;
      FieldInfo[] fields = src.GetType().GetFields(bindingAttr);
      for (int index = 0; index < fields.Length; ++index)
      {
        if (!fields[index].IsStatic)
          fields[index].SetValue(dst, fields[index].GetValue(src));
      }
    }

    public static T AccessInternalField<T>(this Type type, object obj, string memberName)
    {
      if (string.IsNullOrEmpty(memberName) || type == (Type) null)
        return default (T);
      BindingFlags bindingFlags = BindingFlags.NonPublic;
      BindingFlags bindingAttr = obj == null ? bindingFlags | BindingFlags.Static : bindingFlags | BindingFlags.Instance;
      FieldInfo field = type.GetField(memberName, bindingAttr);
      return field != (FieldInfo) null && field.FieldType == typeof (T) ? (T) field.GetValue(obj) : default (T);
    }

    public static object GetParentObject(string path, object obj)
    {
      string[] strArray = path.Split('.');
      if (strArray.Length == 1)
        return obj;
      obj = obj.GetType().GetField(strArray[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(obj);
      return ReflectionHelpers.GetParentObject(string.Join(".", strArray, 1, strArray.Length - 1), obj);
    }

    public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
    {
      if (expr.Body.NodeType != ExpressionType.MemberAccess)
        throw new InvalidOperationException();
      MemberExpression memberExpression = expr.Body as MemberExpression;
      List<string> stringList = new List<string>();
      for (; memberExpression != null; memberExpression = memberExpression.Expression as MemberExpression)
        stringList.Add(memberExpression.Member.Name);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = stringList.Count - 1; index >= 0; --index)
      {
        stringBuilder.Append(stringList[index]);
        if (index > 0)
          stringBuilder.Append('.');
      }
      return stringBuilder.ToString();
    }
  }
}

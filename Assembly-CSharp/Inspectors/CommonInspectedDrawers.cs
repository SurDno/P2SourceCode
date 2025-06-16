// Decompiled with JetBrains decompiler
// Type: Inspectors.CommonInspectedDrawers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Proxies;
using Cofe.Utility;
using Engine.Assets.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Inspectors
{
  [Initialisable]
  public static class CommonInspectedDrawers
  {
    private static void GenerateItemMenu(
      IList contextList,
      int contextIndex,
      IContextMenu menu,
      Action<object> setter)
    {
      Action action1 = (Action) null;
      if (contextIndex != 0)
        action1 = (Action) (() =>
        {
          object context = contextList[contextIndex - 1];
          contextList[contextIndex - 1] = contextList[contextIndex];
          contextList[contextIndex] = context;
          Action<object> action2 = setter;
          if (action2 == null)
            return;
          action2(context);
        });
      menu.AddItem("Up", false, action1);
      Action action3 = (Action) null;
      if (contextIndex != contextList.Count - 1)
        action3 = (Action) (() =>
        {
          object context = contextList[contextIndex + 1];
          contextList[contextIndex + 1] = contextList[contextIndex];
          contextList[contextIndex] = context;
          Action<object> action4 = setter;
          if (action4 == null)
            return;
          action4(context);
        });
      menu.AddItem("Down", false, action3);
      menu.AddSeparator("");
      menu.AddItem("Remove", false, (Action) (() =>
      {
        contextList.RemoveAt(contextIndex);
        Action<object> action5 = setter;
        if (action5 == null)
          return;
        action5((object) null);
      }));
    }

    private static void GenerateClassMenu(
      object contextObject,
      int contextIndex,
      Type baseType,
      Type type,
      object value,
      object target,
      MemberInfo member,
      IContextMenu menu,
      Action<object> setter)
    {
      Action action1 = (Action) null;
      ICloneable cloneable = value as ICloneable;
      if (cloneable != null)
        action1 = (Action) (() => InspectedDrawerService.CopyPasteObject = cloneable.Clone());
      menu.AddItem("Copy", false, action1);
      Action action2 = (Action) null;
      object copyObject = InspectedDrawerService.CopyPasteObject;
      if (copyObject != null && TypeUtility.IsAssignableFrom(baseType, copyObject.GetType()))
        action2 = (Action) (() =>
        {
          object obj = ((ICloneable) copyObject).Clone();
          Action<object> action3 = setter;
          if (action3 == null)
            return;
          action3(obj);
        });
      menu.AddItem("Paste", false, action2);
      menu.AddSeparator("");
      Action action4 = (Action) (() =>
      {
        object obj = (object) null;
        Action<object> action5 = setter;
        if (action5 == null)
          return;
        action5(obj);
      });
      menu.AddItem("null", value == null, action4);
      foreach (Type derivedType1 in DerivedTypeService.GetDerivedTypes(baseType))
      {
        Type derivedType = derivedType1;
        Action action6 = (Action) (() =>
        {
          object obj = ProxyFactory.Create(derivedType);
          Action<object> action7 = setter;
          if (action7 == null)
            return;
          action7(obj);
        });
        Type type1 = ProxyFactory.GetType(type);
        bool on = value != null && type1 == derivedType;
        menu.AddItem(TypeNameService.GetTypeName(derivedType, true), on, action6);
      }
    }

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InspectedDrawerService.Add<Type>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        Type type1 = (Type) value;
        drawer.TextField(name, TypeUtility.GetTypeFullName(type1, false));
      }));
      InspectedDrawerService.Add<int>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        int num1 = (int) value;
        int num2 = drawer.IntField(name, num1);
        if (!mutable || num1 == num2)
          return;
        value = (object) num2;
        if (setter != null)
          setter(value);
      }));
      InspectedDrawerService.Add<uint>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = value.ToString();
        drawer.TextField(name, str);
      }));
      InspectedDrawerService.Add<long>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        long num3 = (long) value;
        long num4 = drawer.LongField(name, num3);
        if (!mutable || num3 == num4)
          return;
        value = (object) num4;
        if (setter != null)
          setter(value);
      }));
      InspectedDrawerService.Add<ulong>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = value.ToString();
        drawer.TextField(name, str);
      }));
      InspectedDrawerService.Add<float>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        float num5 = (float) value;
        SliderAttribute customAttribute = member.GetCustomAttribute<SliderAttribute>();
        float num6 = customAttribute == null ? drawer.FloatField(name, num5) : drawer.SliderField(name, num5, customAttribute.Min, customAttribute.Max);
        if (!mutable || (double) num5 == (double) num6)
          return;
        value = (object) num6;
        if (setter != null)
          setter(value);
      }));
      InspectedDrawerService.Add<double>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        double num7 = (double) value;
        double num8 = drawer.DoubleField(name, num7);
        if (!mutable || num7 == num8)
          return;
        value = (object) num8;
        if (setter != null)
          setter(value);
      }));
      InspectedDrawerService.Add<bool>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        bool flag1 = (bool) value;
        bool flag2 = drawer.BoolField(name, flag1);
        if (!mutable || flag1 == flag2)
          return;
        value = (object) flag2;
        if (setter != null)
          setter(value);
      }));
      InspectedDrawerService.Add<string>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str1 = (string) value;
        string str2 = member.GetCustomAttribute<MultiAttribute>() == null ? drawer.TextField(name, str1) : drawer.MultiTextField(name, str1);
        if (!mutable || !(str1 != str2))
          return;
        value = (object) str2;
        if (setter != null)
          setter(value);
      }));
      InspectedDrawerService.Add<Guid>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = value.ToString();
        drawer.TextField(name, str);
      }));
      InspectedDrawerService.Add<DateTime>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = ((DateTime) value).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        drawer.TextField(name, str);
      }));
      InspectedDrawerService.Add<TimeSpan>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        double totalSeconds = ((TimeSpan) value).TotalSeconds;
        double num = drawer.DoubleField(name, totalSeconds);
        if (!mutable || num == totalSeconds)
          return;
        value = (object) TimeSpan.FromSeconds(num);
        if (setter != null)
          setter(value);
      }));
      InspectedDrawerService.Add(typeof (KeyValuePair<,>), (InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = name;
        Type genericArgument1 = type.GetGenericArguments()[0];
        object obj1 = type.GetProperty("Key").GetValue(value, (object[]) null);
        string displayName = str + " (" + obj1.ToString() + ")";
        InspectedDrawerUtility.Foldout fold = InspectedDrawerUtility.BeginComplex(name, displayName, (IExpandedProvider) context, drawer);
        if (fold.Expand)
        {
          context.DrawInspected("Key", genericArgument1, obj1, mutable, target, member, (Action<object>) (value2 =>
          {
            Action<object> action = setter;
            if (action == null)
              return;
            action(value);
          }));
          Type genericArgument2 = type.GetGenericArguments()[1];
          object obj2 = type.GetProperty("Value").GetValue(value, (object[]) null);
          context.DrawInspected("Value", genericArgument2, obj2, mutable, target, member, (Action<object>) (value2 =>
          {
            Action<object> action = setter;
            if (action == null)
              return;
            action(value);
          }));
        }
        InspectedDrawerUtility.EndComplex(fold, name, (IExpandedProvider) context, drawer);
      }));
      InspectedDrawerService.AddConditional((Func<Type, bool>) (type => type.IsEnum), (InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        Enum enum1 = (Enum) value;
        Enum enum2 = member.GetCustomAttribute<MaskAttribute>() == null ? (member.GetCustomAttribute<SortedAttribute>() == null ? drawer.EnumPopup(name, enum1, (IExpandedProvider) context) : drawer.EnumSortedPopup(name, enum1, (IExpandedProvider) context)) : drawer.EnumMaskPopup(name, enum1, (IExpandedProvider) context);
        if (!mutable || enum1.Equals((object) enum2))
          return;
        value = (object) enum2;
        if (setter != null)
          setter(value);
      }));
      InspectedDrawerService.AddConditional((Func<Type, bool>) (type => typeof (IList).IsAssignableFrom(type)), (InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = name;
        IList list = (IList) value;
        Type baseType = type;
        int contextIndex = context.ContextIndex;
        object contextObject = context.ContextObject;
        Action contextItemMenu = context.ContextItemMenu;
        context.ContextIndex = -1;
        context.ContextItemMenu = (Action) null;
        Action contextMenu = (Action) null;
        string displayName;
        if (value != null)
          displayName = str + " [" + (object) list.Count + "]";
        else
          displayName = str + " [null]";
        if (mutable)
          contextMenu = (Action) (() =>
          {
            Type elementType = InspectedDrawerService.GetElementType(type);
            IContextMenu menu = drawer.CreateMenu();
            if (value != null)
            {
              Action action9 = (Action) (() =>
              {
                list.Add(TypeDefaultUtility.GetDefault(elementType));
                Action<object> action2 = setter;
                if (action2 == null)
                  return;
                action2((object) list);
              });
              menu.AddItem("Add", false, action9);
              menu.AddSeparator("");
              Action action10 = (Action) null;
              if (list.Count > 0)
                action10 = (Action) (() =>
                {
                  list.RemoveAt(0);
                  Action<object> action4 = setter;
                  if (action4 == null)
                    return;
                  action4((object) list);
                });
              menu.AddItem("Remove First", false, action10);
              Action action11 = (Action) null;
              if (list.Count > 0)
                action11 = (Action) (() =>
                {
                  list.RemoveAt(list.Count - 1);
                  Action<object> action6 = setter;
                  if (action6 == null)
                    return;
                  action6((object) list);
                });
              menu.AddItem("Remove Last", false, action11);
              Action action12 = (Action) null;
              if (list.Count > 0)
                action12 = (Action) (() =>
                {
                  list.Clear();
                  Action<object> action8 = setter;
                  if (action8 == null)
                    return;
                  action8((object) list);
                });
              menu.AddItem("Clear", false, action12);
            }
            CommonInspectedDrawers.GenerateClassMenu(contextObject, contextIndex, baseType, type, value, target, member, menu, (Action<object>) (value2 =>
            {
              Action<object> action = setter;
              if (action == null)
                return;
              action(value2);
            }));
            menu.Show();
          });
        InspectedDrawerUtility.Foldout fold = InspectedDrawerUtility.BeginComplex(name, displayName, (IExpandedProvider) context, drawer, contextMenu);
        if (fold.Expand && list != null)
        {
          Type elementType = InspectedDrawerService.GetElementType(type);
          for (int index = 0; index < list.Count; ++index)
          {
            object obj = list[index];
            int level = drawer.BeginBox();
            string name1 = "Element " + (object) index;
            context.ContextIndex = index;
            int captureIndex = index;
            context.DrawInspected(name1, elementType, obj, mutable, target, member, (Action<object>) (value2 =>
            {
              if (captureIndex < list.Count)
                list[captureIndex] = value2;
              Action<object> action = setter;
              if (action == null)
                return;
              action((object) list);
            }));
            drawer.EndBox(level);
          }
        }
        InspectedDrawerUtility.EndComplex(fold, name, (IExpandedProvider) context, drawer);
      }));
      InspectedDrawerService.AddConditional((Func<Type, bool>) (type => typeof (IEnumerable).IsAssignableFrom(type)), (InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = name;
        IEnumerable data = (IEnumerable) value;
        int contextIndex = context.ContextIndex;
        object contextObject = context.ContextObject;
        Action contextItemMenu = context.ContextItemMenu;
        context.ContextIndex = -1;
        context.ContextItemMenu = (Action) null;
        Action contextMenu = (Action) null;
        string displayName;
        if (data != null)
        {
          int num = 0;
          if (data is ICollection collection2)
          {
            num = collection2.Count;
          }
          else
          {
            foreach (object obj in data)
              ++num;
          }
          displayName = str + " [" + (object) num + "]";
        }
        else
          displayName = str + " [null]";
        InspectedDrawerUtility.Foldout fold = InspectedDrawerUtility.BeginComplex(name, displayName, (IExpandedProvider) context, drawer, contextMenu);
        if (fold.Expand && data != null)
        {
          Type elementType = InspectedDrawerService.GetElementType(type);
          int num = 0;
          foreach (object obj in data)
          {
            int level = drawer.BeginBox();
            string name2 = "Element " + (object) num;
            context.ContextIndex = num;
            context.DrawInspected(name2, elementType, obj, mutable, target, member, (Action<object>) (value2 =>
            {
              Action<object> action = setter;
              if (action == null)
                return;
              action((object) data);
            }));
            drawer.EndBox(level);
            ++num;
          }
        }
        InspectedDrawerUtility.EndComplex(fold, name, (IExpandedProvider) context, drawer);
      }));
      InspectedDrawerService.Add<object>((InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        Type baseType = type;
        context.ElementName = "";
        if (value != null)
        {
          type = value.GetType();
          MetaService.GetContainer(type).GetHandler(context.NameId).Compute(value, (object) context);
        }
        else
          context.ElementName = "null";
        int contextIndex = context.ContextIndex;
        object contextObject = context.ContextObject;
        Action contextItemMenu = context.ContextItemMenu;
        context.ContextIndex = -1;
        context.ContextItemMenu = (Action) null;
        Action contextMenu = (Action) null;
        if (mutable)
          contextMenu = contextItemMenu == null ? (Action) (() =>
          {
            IContextMenu menu3 = drawer.CreateMenu();
            if (member.GetValue(target) is IList contextList2)
            {
              int contextIndex1 = contextIndex;
              IContextMenu menu4 = menu3;
              CommonInspectedDrawers.GenerateItemMenu(contextList2, contextIndex1, menu4, (Action<object>) (value2 =>
              {
                Action<object> action = setter;
                if (action == null)
                  return;
                action(value2);
              }));
              menu3.AddSeparator("");
            }
            CommonInspectedDrawers.GenerateClassMenu(contextObject, contextIndex, baseType, type, value, target, member, menu3, (Action<object>) (value2 =>
            {
              Action<object> action = setter;
              if (action == null)
                return;
              action(value2);
            }));
            menu3.Show();
          }) : contextItemMenu;
        string str = name;
        string displayName;
        if (string.IsNullOrEmpty(context.ElementName))
          displayName = str + " (" + (value != null ? TypeNameService.GetTypeName(value.GetType()) : "") + ")";
        else
          displayName = str + " (" + (value != null ? TypeNameService.GetTypeName(value.GetType()) + " : " : "") + context.ElementName + ")";
        InspectedDrawerUtility.Foldout fold = InspectedDrawerUtility.BeginComplex(name, displayName, (IExpandedProvider) context, drawer, contextMenu);
        if (fold.Expand && value != null)
          MetaService.GetContainer(type).GetHandler(context.DrawId).Compute(value, (object) new InspectedContext()
          {
            Provider = context,
            Setter = (Action<object>) (value2 =>
            {
              Action<object> action = setter;
              if (action == null)
                return;
              action(value);
            })
          });
        InspectedDrawerUtility.EndComplex(fold, name, (IExpandedProvider) context, drawer);
      }));
    }
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Cofe.Meta;
using Cofe.Proxies;
using Cofe.Utility;
using Engine.Assets.Internal;

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
      Action action1 = null;
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
      Action action3 = null;
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
        action5(null);
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
      Action action1 = null;
      ICloneable cloneable = value as ICloneable;
      if (cloneable != null)
        action1 = () => InspectedDrawerService.CopyPasteObject = cloneable.Clone();
      menu.AddItem("Copy", false, action1);
      Action action2 = null;
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
        object obj = null;
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

    [Initialise]
    private static void Initialise()
    {
      InspectedDrawerService.Add<Type>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        Type type1 = (Type) value;
        drawer.TextField(name, TypeUtility.GetTypeFullName(type1, false));
      });
      InspectedDrawerService.Add<int>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        int num1 = (int) value;
        int num2 = drawer.IntField(name, num1);
        if (!mutable || num1 == num2)
          return;
        value = num2;
        if (setter != null)
          setter(value);
      });
      InspectedDrawerService.Add<uint>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = value.ToString();
        drawer.TextField(name, str);
      });
      InspectedDrawerService.Add<long>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        long num3 = (long) value;
        long num4 = drawer.LongField(name, num3);
        if (!mutable || num3 == num4)
          return;
        value = num4;
        if (setter != null)
          setter(value);
      });
      InspectedDrawerService.Add<ulong>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = value.ToString();
        drawer.TextField(name, str);
      });
      InspectedDrawerService.Add<float>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        float num5 = (float) value;
        SliderAttribute customAttribute = member.GetCustomAttribute<SliderAttribute>();
        float num6 = customAttribute == null ? drawer.FloatField(name, num5) : drawer.SliderField(name, num5, customAttribute.Min, customAttribute.Max);
        if (!mutable || num5 == (double) num6)
          return;
        value = num6;
        if (setter != null)
          setter(value);
      });
      InspectedDrawerService.Add<double>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        double num7 = (double) value;
        double num8 = drawer.DoubleField(name, num7);
        if (!mutable || num7 == num8)
          return;
        value = num8;
        if (setter != null)
          setter(value);
      });
      InspectedDrawerService.Add<bool>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        bool flag1 = (bool) value;
        bool flag2 = drawer.BoolField(name, flag1);
        if (!mutable || flag1 == flag2)
          return;
        value = flag2;
        if (setter != null)
          setter(value);
      });
      InspectedDrawerService.Add<string>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str1 = (string) value;
        string str2 = member.GetCustomAttribute<MultiAttribute>() == null ? drawer.TextField(name, str1) : drawer.MultiTextField(name, str1);
        if (!mutable || !(str1 != str2))
          return;
        value = str2;
        if (setter != null)
          setter(value);
      });
      InspectedDrawerService.Add<Guid>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = value.ToString();
        drawer.TextField(name, str);
      });
      InspectedDrawerService.Add<DateTime>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = ((DateTime) value).ToString(CultureInfo.InvariantCulture);
        drawer.TextField(name, str);
      });
      InspectedDrawerService.Add<TimeSpan>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        double totalSeconds = ((TimeSpan) value).TotalSeconds;
        double num = drawer.DoubleField(name, totalSeconds);
        if (!mutable || num == totalSeconds)
          return;
        value = TimeSpan.FromSeconds(num);
        if (setter != null)
          setter(value);
      });
      InspectedDrawerService.Add(typeof (KeyValuePair<,>), (name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = name;
        Type genericArgument1 = type.GetGenericArguments()[0];
        object obj1 = type.GetProperty("Key").GetValue(value, null);
        string displayName = str + " (" + obj1 + ")";
        InspectedDrawerUtility.Foldout fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer);
        if (fold.Expand)
        {
          context.DrawInspected("Key", genericArgument1, obj1, mutable, target, member, value2 =>
          {
            Action<object> action = setter;
            if (action == null)
              return;
            action(value);
          });
          Type genericArgument2 = type.GetGenericArguments()[1];
          object obj2 = type.GetProperty("Value").GetValue(value, null);
          context.DrawInspected("Value", genericArgument2, obj2, mutable, target, member, value2 =>
          {
            Action<object> action = setter;
            if (action == null)
              return;
            action(value);
          });
        }
        InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
      });
      InspectedDrawerService.AddConditional(type => type.IsEnum, (name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        Enum enum1 = (Enum) value;
        Enum enum2 = member.GetCustomAttribute<MaskAttribute>() == null ? (member.GetCustomAttribute<SortedAttribute>() == null ? drawer.EnumPopup(name, enum1, context) : drawer.EnumSortedPopup(name, enum1, context)) : drawer.EnumMaskPopup(name, enum1, context);
        if (!mutable || enum1.Equals(enum2))
          return;
        value = enum2;
        if (setter != null)
          setter(value);
      });
      InspectedDrawerService.AddConditional(type => typeof (IList).IsAssignableFrom(type), (name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = name;
        IList list = (IList) value;
        Type baseType = type;
        int contextIndex = context.ContextIndex;
        object contextObject = context.ContextObject;
        Action contextItemMenu = context.ContextItemMenu;
        context.ContextIndex = -1;
        context.ContextItemMenu = null;
        Action contextMenu = null;
        string displayName;
        if (value != null)
          displayName = str + " [" + list.Count + "]";
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
                action2(list);
              });
              menu.AddItem("Add", false, action9);
              menu.AddSeparator("");
              Action action10 = null;
              if (list.Count > 0)
                action10 = (Action) (() =>
                {
                  list.RemoveAt(0);
                  Action<object> action4 = setter;
                  if (action4 == null)
                    return;
                  action4(list);
                });
              menu.AddItem("Remove First", false, action10);
              Action action11 = null;
              if (list.Count > 0)
                action11 = (Action) (() =>
                {
                  list.RemoveAt(list.Count - 1);
                  Action<object> action6 = setter;
                  if (action6 == null)
                    return;
                  action6(list);
                });
              menu.AddItem("Remove Last", false, action11);
              Action action12 = null;
              if (list.Count > 0)
                action12 = (Action) (() =>
                {
                  list.Clear();
                  Action<object> action8 = setter;
                  if (action8 == null)
                    return;
                  action8(list);
                });
              menu.AddItem("Clear", false, action12);
            }
            GenerateClassMenu(contextObject, contextIndex, baseType, type, value, target, member, menu, value2 =>
            {
              Action<object> action = setter;
              if (action == null)
                return;
              action(value2);
            });
            menu.Show();
          });
        InspectedDrawerUtility.Foldout fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer, contextMenu);
        if (fold.Expand && list != null)
        {
          Type elementType = InspectedDrawerService.GetElementType(type);
          for (int index = 0; index < list.Count; ++index)
          {
            object obj = list[index];
            int level = drawer.BeginBox();
            string name1 = "Element " + index;
            context.ContextIndex = index;
            int captureIndex = index;
            context.DrawInspected(name1, elementType, obj, mutable, target, member, value2 =>
            {
              if (captureIndex < list.Count)
                list[captureIndex] = value2;
              Action<object> action = setter;
              if (action == null)
                return;
              action(list);
            });
            drawer.EndBox(level);
          }
        }
        InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
      });
      InspectedDrawerService.AddConditional(type => typeof (IEnumerable).IsAssignableFrom(type), (name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        string str = name;
        IEnumerable data = (IEnumerable) value;
        int contextIndex = context.ContextIndex;
        object contextObject = context.ContextObject;
        Action contextItemMenu = context.ContextItemMenu;
        context.ContextIndex = -1;
        context.ContextItemMenu = null;
        Action contextMenu = null;
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
          displayName = str + " [" + num + "]";
        }
        else
          displayName = str + " [null]";
        InspectedDrawerUtility.Foldout fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer, contextMenu);
        if (fold.Expand && data != null)
        {
          Type elementType = InspectedDrawerService.GetElementType(type);
          int num = 0;
          foreach (object obj in data)
          {
            int level = drawer.BeginBox();
            string name2 = "Element " + num;
            context.ContextIndex = num;
            context.DrawInspected(name2, elementType, obj, mutable, target, member, value2 =>
            {
              Action<object> action = setter;
              if (action == null)
                return;
              action(data);
            });
            drawer.EndBox(level);
            ++num;
          }
        }
        InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
      });
      InspectedDrawerService.Add<object>((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        Type baseType = type;
        context.ElementName = "";
        if (value != null)
        {
          type = value.GetType();
          MetaService.GetContainer(type).GetHandler(context.NameId).Compute(value, context);
        }
        else
          context.ElementName = "null";
        int contextIndex = context.ContextIndex;
        object contextObject = context.ContextObject;
        Action contextItemMenu = context.ContextItemMenu;
        context.ContextIndex = -1;
        context.ContextItemMenu = null;
        Action contextMenu = null;
        if (mutable)
          contextMenu = contextItemMenu == null ? (Action) (() =>
          {
            IContextMenu menu3 = drawer.CreateMenu();
            if (member.GetValue(target) is IList contextList2)
            {
              int contextIndex1 = contextIndex;
              IContextMenu menu4 = menu3;
              GenerateItemMenu(contextList2, contextIndex1, menu4, value2 =>
              {
                Action<object> action = setter;
                if (action == null)
                  return;
                action(value2);
              });
              menu3.AddSeparator("");
            }
            GenerateClassMenu(contextObject, contextIndex, baseType, type, value, target, member, menu3, value2 =>
            {
              Action<object> action = setter;
              if (action == null)
                return;
              action(value2);
            });
            menu3.Show();
          }) : contextItemMenu;
        string str = name;
        string displayName;
        if (string.IsNullOrEmpty(context.ElementName))
          displayName = str + " (" + (value != null ? TypeNameService.GetTypeName(value.GetType()) : "") + ")";
        else
          displayName = str + " (" + (value != null ? TypeNameService.GetTypeName(value.GetType()) + " : " : "") + context.ElementName + ")";
        InspectedDrawerUtility.Foldout fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer, contextMenu);
        if (fold.Expand && value != null)
          MetaService.GetContainer(type).GetHandler(context.DrawId).Compute(value, new InspectedContext {
            Provider = context,
            Setter = value2 =>
            {
              Action<object> action = setter;
              if (action == null)
                return;
              action(value);
            }
          });
        InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
      });
    }
  }
}

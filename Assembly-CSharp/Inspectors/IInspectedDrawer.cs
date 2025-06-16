// Decompiled with JetBrains decompiler
// Type: Inspectors.IInspectedDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Inspectors
{
  public interface IInspectedDrawer
  {
    int IndentLevel { get; set; }

    float IndentSize { get; }

    string MultiTextField(string name, string value);

    string TextField(string name, string value);

    int IntField(string name, int value);

    long LongField(string name, long value);

    float FloatField(string name, float value);

    float SliderField(string name, float value, float min, float max);

    double DoubleField(string name, double value);

    bool BoolField(string name, bool value);

    Vector2 Vector2Field(string name, Vector2 value);

    Vector3 Vector3Field(string name, Vector3 value);

    Vector4 Vector4Field(string name, Vector4 value);

    Color ColorField(string name, Color value);

    Rect RectField(string name, Rect value);

    Bounds BoundsField(string name, Bounds value);

    UnityEngine.Object ObjectField(string name, UnityEngine.Object value, System.Type type);

    Enum EnumPopup(string name, Enum value, IExpandedProvider context);

    Enum EnumSortedPopup(string name, Enum value, IExpandedProvider context);

    Enum EnumMaskPopup(string name, Enum value, IExpandedProvider context);

    string ListPopup(string name, string value, string[] values, IExpandedProvider context);

    bool Foldout(string name, bool value, Action context);

    bool ButtonField(string name);

    AnimationCurve CurveField(string name, AnimationCurve value);

    int BeginBox();

    void EndBox(int level);

    IContextMenu CreateMenu();
  }
}

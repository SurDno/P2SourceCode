// Decompiled with JetBrains decompiler
// Type: AmplifyColor.VolumeEffectField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffectField
  {
    public string fieldName;
    public string fieldType;
    public float valueSingle;
    public Color valueColor;
    public bool valueBoolean;
    public Vector2 valueVector2;
    public Vector3 valueVector3;
    public Vector4 valueVector4;

    public VolumeEffectField(string fieldName, string fieldType)
    {
      this.fieldName = fieldName;
      this.fieldType = fieldType;
    }

    public VolumeEffectField(FieldInfo pi, Component c)
      : this(pi.Name, pi.FieldType.FullName)
    {
      this.UpdateValue(pi.GetValue((object) c));
    }

    public static bool IsValidType(string type)
    {
      string str = type;
      return str == "System.Single" || str == "System.Boolean" || str == "UnityEngine.Color" || str == "UnityEngine.Vector2" || str == "UnityEngine.Vector3" || str == "UnityEngine.Vector4";
    }

    public void UpdateValue(object val)
    {
      switch (this.fieldType)
      {
        case "System.Single":
          this.valueSingle = (float) val;
          break;
        case "System.Boolean":
          this.valueBoolean = (bool) val;
          break;
        case "UnityEngine.Color":
          this.valueColor = (Color) val;
          break;
        case "UnityEngine.Vector2":
          this.valueVector2 = (Vector2) val;
          break;
        case "UnityEngine.Vector3":
          this.valueVector3 = (Vector3) val;
          break;
        case "UnityEngine.Vector4":
          this.valueVector4 = (Vector4) val;
          break;
      }
    }
  }
}

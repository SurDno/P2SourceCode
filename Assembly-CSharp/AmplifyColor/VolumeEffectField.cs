using System;
using System.Reflection;
using UnityEngine;

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
      UpdateValue(pi.GetValue(c));
    }

    public static bool IsValidType(string type)
    {
      string str = type;
      return str == "System.Single" || str == "System.Boolean" || str == "UnityEngine.Color" || str == "UnityEngine.Vector2" || str == "UnityEngine.Vector3" || str == "UnityEngine.Vector4";
    }

    public void UpdateValue(object val)
    {
      switch (fieldType)
      {
        case "System.Single":
          valueSingle = (float) val;
          break;
        case "System.Boolean":
          valueBoolean = (bool) val;
          break;
        case "UnityEngine.Color":
          valueColor = (Color) val;
          break;
        case "UnityEngine.Vector2":
          valueVector2 = (Vector2) val;
          break;
        case "UnityEngine.Vector3":
          valueVector3 = (Vector3) val;
          break;
        case "UnityEngine.Vector4":
          valueVector4 = (Vector4) val;
          break;
      }
    }
  }
}

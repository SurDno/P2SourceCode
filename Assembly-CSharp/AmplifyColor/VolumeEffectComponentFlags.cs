using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffectComponentFlags(string name) {
    public string componentName = name;
    public List<VolumeEffectFieldFlags> componentFields = [];
    public bool blendFlag;

    public VolumeEffectComponentFlags(VolumeEffectComponent comp)
      : this(comp.componentName)
    {
      blendFlag = true;
      foreach (VolumeEffectField field in comp.fields)
      {
        if (VolumeEffectField.IsValidType(field.fieldType))
          componentFields.Add(new VolumeEffectFieldFlags(field));
      }
    }

    public VolumeEffectComponentFlags(Component c)
      : this(string.Concat(c.GetType()))
    {
      foreach (FieldInfo field in c.GetType().GetFields())
      {
        if (VolumeEffectField.IsValidType(field.FieldType.FullName))
          componentFields.Add(new VolumeEffectFieldFlags(field));
      }
    }

    public void UpdateComponentFlags(VolumeEffectComponent comp)
    {
      foreach (VolumeEffectField field1 in comp.fields)
      {
        VolumeEffectField field = field1;
        if (componentFields.Find(s => s.fieldName == field.fieldName) == null && VolumeEffectField.IsValidType(field.fieldType))
          componentFields.Add(new VolumeEffectFieldFlags(field));
      }
    }

    public void UpdateComponentFlags(Component c)
    {
      foreach (FieldInfo field in c.GetType().GetFields())
      {
        FieldInfo pi = field;
        if (!componentFields.Exists(s => s.fieldName == pi.Name) && VolumeEffectField.IsValidType(pi.FieldType.FullName))
          componentFields.Add(new VolumeEffectFieldFlags(pi));
      }
    }

    public string[] GetFieldNames()
    {
      return componentFields.Where(r => r.blendFlag).Select(r => r.fieldName).ToArray();
    }
  }
}

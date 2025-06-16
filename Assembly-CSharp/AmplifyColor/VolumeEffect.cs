using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffect
  {
    public AmplifyColorBase gameObject;
    public List<VolumeEffectComponent> components;

    public VolumeEffect(AmplifyColorBase effect)
    {
      gameObject = effect;
      components = new List<VolumeEffectComponent>();
    }

    public static VolumeEffect BlendValuesToVolumeEffect(
      VolumeEffectFlags flags,
      VolumeEffect volume1,
      VolumeEffect volume2,
      float blend)
    {
      VolumeEffect volumeEffect = new VolumeEffect(volume1.gameObject);
      foreach (VolumeEffectComponentFlags component in flags.components)
      {
        if (component.blendFlag)
        {
          VolumeEffectComponent effectComponent1 = volume1.FindEffectComponent(component.componentName);
          VolumeEffectComponent effectComponent2 = volume2.FindEffectComponent(component.componentName);
          if (effectComponent1 != null && effectComponent2 != null)
          {
            VolumeEffectComponent volumeEffectComponent = new VolumeEffectComponent(effectComponent1.componentName);
            foreach (VolumeEffectFieldFlags componentField in component.componentFields)
            {
              if (componentField.blendFlag)
              {
                VolumeEffectField effectField1 = effectComponent1.FindEffectField(componentField.fieldName);
                VolumeEffectField effectField2 = effectComponent2.FindEffectField(componentField.fieldName);
                if (effectField1 != null && effectField2 != null)
                {
                  VolumeEffectField volumeEffectField = new VolumeEffectField(effectField1.fieldName, effectField1.fieldType);
                  switch (volumeEffectField.fieldType)
                  {
                    case "System.Single":
                      volumeEffectField.valueSingle = Mathf.Lerp(effectField1.valueSingle, effectField2.valueSingle, blend);
                      break;
                    case "System.Boolean":
                      volumeEffectField.valueBoolean = effectField2.valueBoolean;
                      break;
                    case "UnityEngine.Vector2":
                      volumeEffectField.valueVector2 = Vector2.Lerp(effectField1.valueVector2, effectField2.valueVector2, blend);
                      break;
                    case "UnityEngine.Vector3":
                      volumeEffectField.valueVector3 = Vector3.Lerp(effectField1.valueVector3, effectField2.valueVector3, blend);
                      break;
                    case "UnityEngine.Vector4":
                      volumeEffectField.valueVector4 = Vector4.Lerp(effectField1.valueVector4, effectField2.valueVector4, blend);
                      break;
                    case "UnityEngine.Color":
                      volumeEffectField.valueColor = Color.Lerp(effectField1.valueColor, effectField2.valueColor, blend);
                      break;
                  }
                  volumeEffectComponent.fields.Add(volumeEffectField);
                }
              }
            }
            volumeEffect.components.Add(volumeEffectComponent);
          }
        }
      }
      return volumeEffect;
    }

    public VolumeEffectComponent AddComponent(Component c, VolumeEffectComponentFlags compFlags)
    {
      if (compFlags == null)
      {
        VolumeEffectComponent volumeEffectComponent = new VolumeEffectComponent(string.Concat(c.GetType()));
        components.Add(volumeEffectComponent);
        return volumeEffectComponent;
      }
      VolumeEffectComponent effectComponent;
      if ((effectComponent = FindEffectComponent(string.Concat(c.GetType()))) != null)
      {
        effectComponent.UpdateComponent(c, compFlags);
        return effectComponent;
      }
      VolumeEffectComponent volumeEffectComponent1 = new VolumeEffectComponent(c, compFlags);
      components.Add(volumeEffectComponent1);
      return volumeEffectComponent1;
    }

    public void RemoveEffectComponent(VolumeEffectComponent comp) => components.Remove(comp);

    public void UpdateVolume()
    {
      if (gameObject == null)
        return;
      foreach (VolumeEffectComponentFlags component1 in gameObject.EffectFlags.components)
      {
        if (component1.blendFlag)
        {
          Component component2 = gameObject.GetComponent(component1.componentName);
          if (component2 != null)
            AddComponent(component2, component1);
        }
      }
    }

    public void SetValues(AmplifyColorBase targetColor)
    {
      VolumeEffectFlags effectFlags = targetColor.EffectFlags;
      GameObject gameObject = targetColor.gameObject;
      foreach (VolumeEffectComponentFlags component1 in effectFlags.components)
      {
        if (component1.blendFlag)
        {
          Component component2 = gameObject.GetComponent(component1.componentName);
          VolumeEffectComponent effectComponent = FindEffectComponent(component1.componentName);
          if (!(component2 == null) && effectComponent != null)
          {
            foreach (VolumeEffectFieldFlags componentField in component1.componentFields)
            {
              if (componentField.blendFlag)
              {
                FieldInfo field = component2.GetType().GetField(componentField.fieldName);
                VolumeEffectField effectField = effectComponent.FindEffectField(componentField.fieldName);
                if (!(field == null) && effectField != null)
                {
                  switch (field.FieldType.FullName)
                  {
                    case "System.Single":
                      field.SetValue(component2, effectField.valueSingle);
                      break;
                    case "System.Boolean":
                      field.SetValue(component2, effectField.valueBoolean);
                      break;
                    case "UnityEngine.Vector2":
                      field.SetValue(component2, effectField.valueVector2);
                      break;
                    case "UnityEngine.Vector3":
                      field.SetValue(component2, effectField.valueVector3);
                      break;
                    case "UnityEngine.Vector4":
                      field.SetValue(component2, effectField.valueVector4);
                      break;
                    case "UnityEngine.Color":
                      field.SetValue(component2, effectField.valueColor);
                      break;
                  }
                }
              }
            }
          }
        }
      }
    }

    public void BlendValues(AmplifyColorBase targetColor, VolumeEffect other, float blendAmount)
    {
      VolumeEffectFlags effectFlags = targetColor.EffectFlags;
      GameObject gameObject = targetColor.gameObject;
      for (int index1 = 0; index1 < effectFlags.components.Count; ++index1)
      {
        VolumeEffectComponentFlags component1 = effectFlags.components[index1];
        if (component1.blendFlag)
        {
          Component component2 = gameObject.GetComponent(component1.componentName);
          VolumeEffectComponent effectComponent1 = FindEffectComponent(component1.componentName);
          VolumeEffectComponent effectComponent2 = other.FindEffectComponent(component1.componentName);
          if (!(component2 == null) && effectComponent1 != null && effectComponent2 != null)
          {
            for (int index2 = 0; index2 < component1.componentFields.Count; ++index2)
            {
              VolumeEffectFieldFlags componentField = component1.componentFields[index2];
              if (componentField.blendFlag)
              {
                FieldInfo field = component2.GetType().GetField(componentField.fieldName);
                VolumeEffectField effectField1 = effectComponent1.FindEffectField(componentField.fieldName);
                VolumeEffectField effectField2 = effectComponent2.FindEffectField(componentField.fieldName);
                if (!(field == null) && effectField1 != null && effectField2 != null)
                {
                  switch (field.FieldType.FullName)
                  {
                    case "System.Single":
                      field.SetValue(component2, Mathf.Lerp(effectField1.valueSingle, effectField2.valueSingle, blendAmount));
                      break;
                    case "System.Boolean":
                      field.SetValue(component2, effectField2.valueBoolean);
                      break;
                    case "UnityEngine.Vector2":
                      field.SetValue(component2, Vector2.Lerp(effectField1.valueVector2, effectField2.valueVector2, blendAmount));
                      break;
                    case "UnityEngine.Vector3":
                      field.SetValue(component2, Vector3.Lerp(effectField1.valueVector3, effectField2.valueVector3, blendAmount));
                      break;
                    case "UnityEngine.Vector4":
                      field.SetValue(component2, Vector4.Lerp(effectField1.valueVector4, effectField2.valueVector4, blendAmount));
                      break;
                    case "UnityEngine.Color":
                      field.SetValue(component2, Color.Lerp(effectField1.valueColor, effectField2.valueColor, blendAmount));
                      break;
                  }
                }
              }
            }
          }
        }
      }
    }

    public VolumeEffectComponent FindEffectComponent(string compName)
    {
      for (int index = 0; index < components.Count; ++index)
      {
        if (components[index].componentName == compName)
          return components[index];
      }
      return null;
    }

    public static Component[] ListAcceptableComponents(AmplifyColorBase go)
    {
      return go == null ? new Component[0] : go.GetComponents(typeof (Component)).Where(comp => comp != null && !string.Concat(comp.GetType()).StartsWith("UnityEngine.") && !(comp.GetType() == typeof (AmplifyColorBase))).ToArray();
    }

    public string[] GetComponentNames()
    {
      return components.Select(r => r.componentName).ToArray();
    }
  }
}

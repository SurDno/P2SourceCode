﻿using System;
using System.Reflection;
using ParadoxNotion;
using UnityEngine;

namespace NodeCanvas.Framework
{
  [Serializable]
  public class Variable<T> : Variable
  {
    [SerializeField]
    private T _value;
    [SerializeField]
    private string _propertyPath;
    private Func<T> getter;
    private Action<T> setter;

    public override string propertyPath
    {
      get => _propertyPath;
      set => _propertyPath = value;
    }

    public override bool hasBinding => !string.IsNullOrEmpty(_propertyPath);

    protected override object objectValue
    {
      get => value;
      set => this.value = (T) value;
    }

    public override Type varType => typeof (T);

    public T value
    {
      get => getter != null ? getter() : _value;
      set
      {
        if (HasValueChangeEvent())
        {
          if (Equals(_value, value))
            return;
          _value = value;
          Action<T> setter = this.setter;
          if (setter != null)
            setter(value);
          OnValueChanged(name, value);
        }
        else if (this.setter != null)
          this.setter(value);
        else
          _value = value;
      }
    }

    public T GetValue() => value;

    public void SetValue(T newValue) => value = newValue;

    public override void BindProperty(MemberInfo prop, GameObject target = null)
    {
      if ((object) (prop as PropertyInfo) == null && !(prop is FieldInfo))
        return;
      _propertyPath = string.Format("{0}.{1}", prop.RTReflectedType().FullName, prop.Name);
      if (target != null)
        InitializePropertyBinding(target);
    }

    public override void UnBindProperty()
    {
      _propertyPath = null;
      getter = null;
      setter = null;
    }

    public override void InitializePropertyBinding(GameObject go, bool callSetter = false)
    {
      if (!hasBinding || !Application.isPlaying)
        return;
      getter = null;
      setter = null;
      int length = _propertyPath.LastIndexOf('.');
      string typeFullName = _propertyPath.Substring(0, length);
      string name = _propertyPath.Substring(length + 1);
      Type type = ReflectionTools.GetType(typeFullName, true);
      if (type == null)
      {
        Debug.LogError(string.Format("Type '{0}' not found for Blackboard Variable '{1}' Binding", typeFullName, this.name), go);
      }
      else
      {
        PropertyInfo property = type.RTGetProperty(name);
        if (property != null)
        {
          MethodInfo getMethod = property.RTGetGetMethod();
          MethodInfo setMethod = property.RTGetSetMethod();
          bool flag = getMethod != null && getMethod.IsStatic || setMethod != null && setMethod.IsStatic;
          Component instance = flag ? null : go.GetComponent(type);
          if (instance == null && !flag)
          {
            Debug.LogError(string.Format("A Blackboard Variable '{0}' is due to bind to a Component type that is missing '{1}'. Binding ignored", this.name, typeFullName));
          }
          else
          {
            if (property.CanRead)
            {
              try
              {
                getter = getMethod.RTCreateDelegate<Func<T>>(instance);
              }
              catch
              {
                getter = (Func<T>) (() => (T) getMethod.Invoke(instance, null));
              }
            }
            else
              getter = (Func<T>) (() =>
              {
                Debug.LogError(string.Format("You tried to Get a Property Bound Variable '{0}', but the Bound Property '{1}' is Write Only!", this.name, _propertyPath));
                return default (T);
              });
            if (property.CanWrite)
            {
              try
              {
                setter = setMethod.RTCreateDelegate<Action<T>>(instance);
              }
              catch
              {
                setter = o => setMethod.Invoke(instance, [
                  o
                ]);
              }
              if (!callSetter)
                return;
              setter(_value);
            }
            else
              setter = o => Debug.LogError(string.Format("You tried to Set a Property Bound Variable '{0}', but the Bound Property '{1}' is Read Only!", this.name, _propertyPath));
          }
        }
        else
        {
          FieldInfo field = type.RTGetField(name);
          if (field != null)
          {
            Component instance = field.IsStatic ? null : go.GetComponent(type);
            if (instance == null && !field.IsStatic)
              Debug.LogError(string.Format("A Blackboard Variable '{0}' is due to bind to a Component type that is missing '{1}'. Binding ignored", this.name, typeFullName));
            else if (field.IsReadOnly())
            {
              T value = (T) field.GetValue(instance);
              getter = (Func<T>) (() => value);
            }
            else
            {
              getter = (Func<T>) (() => (T) field.GetValue(instance));
              setter = o => field.SetValue(instance, o);
            }
          }
          else
            Debug.LogError(string.Format("A Blackboard Variable '{0}' is due to bind to a property/field named '{1}' that does not exist on type '{2}'. Binding ignored", this.name, name, type.FullName));
        }
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.Variable`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using System;
using System.Reflection;
using UnityEngine;

#nullable disable
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
      get => this._propertyPath;
      set => this._propertyPath = value;
    }

    public override bool hasBinding => !string.IsNullOrEmpty(this._propertyPath);

    protected override object objectValue
    {
      get => (object) this.value;
      set => this.value = (T) value;
    }

    public override System.Type varType => typeof (T);

    public T value
    {
      get => this.getter != null ? this.getter() : this._value;
      set
      {
        if (this.HasValueChangeEvent())
        {
          if (object.Equals((object) this._value, (object) value))
            return;
          this._value = value;
          Action<T> setter = this.setter;
          if (setter != null)
            setter(value);
          this.OnValueChanged(this.name, (object) value);
        }
        else if (this.setter != null)
          this.setter(value);
        else
          this._value = value;
      }
    }

    public T GetValue() => this.value;

    public void SetValue(T newValue) => this.value = newValue;

    public override void BindProperty(MemberInfo prop, GameObject target = null)
    {
      if ((object) (prop as PropertyInfo) == null && !(prop is FieldInfo))
        return;
      this._propertyPath = string.Format("{0}.{1}", (object) prop.RTReflectedType().FullName, (object) prop.Name);
      if ((UnityEngine.Object) target != (UnityEngine.Object) null)
        this.InitializePropertyBinding(target, false);
    }

    public override void UnBindProperty()
    {
      this._propertyPath = (string) null;
      this.getter = (Func<T>) null;
      this.setter = (Action<T>) null;
    }

    public override void InitializePropertyBinding(GameObject go, bool callSetter = false)
    {
      if (!this.hasBinding || !Application.isPlaying)
        return;
      this.getter = (Func<T>) null;
      this.setter = (Action<T>) null;
      int length = this._propertyPath.LastIndexOf('.');
      string typeFullName = this._propertyPath.Substring(0, length);
      string name = this._propertyPath.Substring(length + 1);
      System.Type type = ReflectionTools.GetType(typeFullName, true);
      if (type == (System.Type) null)
      {
        Debug.LogError((object) string.Format("Type '{0}' not found for Blackboard Variable '{1}' Binding", (object) typeFullName, (object) this.name), (UnityEngine.Object) go);
      }
      else
      {
        PropertyInfo property = type.RTGetProperty(name);
        if (property != (PropertyInfo) null)
        {
          MethodInfo getMethod = property.RTGetGetMethod();
          MethodInfo setMethod = property.RTGetSetMethod();
          bool flag = getMethod != (MethodInfo) null && getMethod.IsStatic || setMethod != (MethodInfo) null && setMethod.IsStatic;
          Component instance = flag ? (Component) null : go.GetComponent(type);
          if ((UnityEngine.Object) instance == (UnityEngine.Object) null && !flag)
          {
            Debug.LogError((object) string.Format("A Blackboard Variable '{0}' is due to bind to a Component type that is missing '{1}'. Binding ignored", (object) this.name, (object) typeFullName));
          }
          else
          {
            if (property.CanRead)
            {
              try
              {
                this.getter = getMethod.RTCreateDelegate<Func<T>>((object) instance);
              }
              catch
              {
                this.getter = (Func<T>) (() => (T) getMethod.Invoke((object) instance, (object[]) null));
              }
            }
            else
              this.getter = (Func<T>) (() =>
              {
                Debug.LogError((object) string.Format("You tried to Get a Property Bound Variable '{0}', but the Bound Property '{1}' is Write Only!", (object) this.name, (object) this._propertyPath));
                return default (T);
              });
            if (property.CanWrite)
            {
              try
              {
                this.setter = setMethod.RTCreateDelegate<Action<T>>((object) instance);
              }
              catch
              {
                this.setter = (Action<T>) (o => setMethod.Invoke((object) instance, new object[1]
                {
                  (object) o
                }));
              }
              if (!callSetter)
                return;
              this.setter(this._value);
            }
            else
              this.setter = (Action<T>) (o => Debug.LogError((object) string.Format("You tried to Set a Property Bound Variable '{0}', but the Bound Property '{1}' is Read Only!", (object) this.name, (object) this._propertyPath)));
          }
        }
        else
        {
          FieldInfo field = type.RTGetField(name);
          if (field != (FieldInfo) null)
          {
            Component instance = field.IsStatic ? (Component) null : go.GetComponent(type);
            if ((UnityEngine.Object) instance == (UnityEngine.Object) null && !field.IsStatic)
              Debug.LogError((object) string.Format("A Blackboard Variable '{0}' is due to bind to a Component type that is missing '{1}'. Binding ignored", (object) this.name, (object) typeFullName));
            else if (field.IsReadOnly())
            {
              T value = (T) field.GetValue((object) instance);
              this.getter = (Func<T>) (() => value);
            }
            else
            {
              this.getter = (Func<T>) (() => (T) field.GetValue((object) instance));
              this.setter = (Action<T>) (o => field.SetValue((object) instance, (object) o));
            }
          }
          else
            Debug.LogError((object) string.Format("A Blackboard Variable '{0}' is due to bind to a property/field named '{1}' that does not exist on type '{2}'. Binding ignored", (object) this.name, (object) name, (object) type.FullName));
        }
      }
    }
  }
}

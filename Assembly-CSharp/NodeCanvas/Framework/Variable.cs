using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using System.Reflection;
using UnityEngine;

namespace NodeCanvas.Framework
{
  [SpoofAOT]
  [Serializable]
  public abstract class Variable
  {
    [SerializeField]
    private string _name;
    [SerializeField]
    private string _id;
    [SerializeField]
    private bool _protected;

    public event Action<string> onNameChanged;

    public event Action<string, object> onValueChanged;

    public string name
    {
      get => this._name;
      set
      {
        if (!(this._name != value))
          return;
        this._name = value;
        Action<string> onNameChanged = this.onNameChanged;
        if (onNameChanged != null)
          onNameChanged(value);
      }
    }

    public string ID
    {
      get
      {
        if (string.IsNullOrEmpty(this._id))
          this._id = Guid.NewGuid().ToString();
        return this._id;
      }
    }

    public object value
    {
      get => this.objectValue;
      set => this.objectValue = value;
    }

    public bool isProtected
    {
      get => this._protected;
      set => this._protected = value;
    }

    protected bool HasValueChangeEvent() => this.onValueChanged != null;

    protected void OnValueChanged(string name, object value) => this.onValueChanged(name, value);

    protected abstract object objectValue { get; set; }

    public abstract System.Type varType { get; }

    public abstract bool hasBinding { get; }

    public abstract string propertyPath { get; set; }

    public abstract void BindProperty(MemberInfo prop, GameObject target = null);

    public abstract void UnBindProperty();

    public abstract void InitializePropertyBinding(GameObject go, bool callSetter = false);

    public bool CanConvertTo(System.Type toType) => this.GetGetConverter(toType) != null;

    public Func<object> GetGetConverter(System.Type toType)
    {
      if (toType.RTIsAssignableFrom(this.varType))
        return (Func<object>) (() => this.value);
      if (typeof (IConvertible).RTIsAssignableFrom(toType) && typeof (IConvertible).RTIsAssignableFrom(this.varType))
        return (Func<object>) (() =>
        {
          try
          {
            return Convert.ChangeType(this.value, toType);
          }
          catch
          {
            return !toType.RTIsAbstract() ? Activator.CreateInstance(toType) : (object) null;
          }
        });
      if (toType == typeof (Transform) && this.varType == typeof (GameObject))
        return (Func<object>) (() => this.value != null ? (object) (this.value as GameObject).transform : (object) (Transform) null);
      if (toType == typeof (GameObject) && typeof (Component).RTIsAssignableFrom(this.varType))
        return (Func<object>) (() => this.value != null ? (object) (this.value as Component).gameObject : (object) (GameObject) null);
      if (toType == typeof (Vector3) && this.varType == typeof (GameObject))
        return (Func<object>) (() => (object) (this.value != null ? (this.value as GameObject).transform.position : Vector3.zero));
      if (toType == typeof (Vector3) && this.varType == typeof (Transform))
        return (Func<object>) (() => (object) (this.value != null ? (this.value as Transform).position : Vector3.zero));
      if (toType == typeof (Vector3) && this.varType == typeof (Quaternion))
        return (Func<object>) (() => (object) ((Quaternion) this.value).eulerAngles);
      if (toType == typeof (Quaternion) && this.varType == typeof (Vector3))
        return (Func<object>) (() => (object) Quaternion.Euler((Vector3) this.value));
      if (toType == typeof (Vector3) && this.varType == typeof (Vector2))
        return (Func<object>) (() => (object) (Vector3) (Vector2) this.value);
      return toType == typeof (Vector2) && this.varType == typeof (Vector3) ? (Func<object>) (() => (object) (Vector2) (Vector3) this.value) : (Func<object>) null;
    }

    public bool CanConvertFrom(System.Type fromType) => this.GetSetConverter(fromType) != null;

    public Action<object> GetSetConverter(System.Type fromType)
    {
      if (this.varType.RTIsAssignableFrom(fromType))
        return (Action<object>) (o => this.value = o);
      if (typeof (IConvertible).RTIsAssignableFrom(this.varType) && typeof (IConvertible).RTIsAssignableFrom(fromType))
        return (Action<object>) (o =>
        {
          try
          {
            this.value = Convert.ChangeType(o, this.varType);
          }
          catch
          {
            this.value = !this.varType.RTIsAbstract() ? Activator.CreateInstance(this.varType) : (object) null;
          }
        });
      if (this.varType == typeof (Transform) && fromType == typeof (GameObject))
        return (Action<object>) (o => this.value = o != null ? (object) (o as GameObject).transform : (object) (Transform) null);
      if (this.varType == typeof (GameObject) && typeof (Component).RTIsAssignableFrom(fromType))
        return (Action<object>) (o => this.value = o != null ? (object) (o as Component).gameObject : (object) (GameObject) null);
      if (this.varType == typeof (GameObject) && fromType == typeof (Vector3))
        return (Action<object>) (o =>
        {
          if (this.value == null)
            return;
          (this.value as GameObject).transform.position = (Vector3) o;
        });
      if (this.varType == typeof (Transform) && fromType == typeof (Vector3))
        return (Action<object>) (o =>
        {
          if (this.value == null)
            return;
          (this.value as Transform).position = (Vector3) o;
        });
      if (this.varType == typeof (Vector3) && fromType == typeof (Quaternion))
        return (Action<object>) (o => this.value = (object) ((Quaternion) o).eulerAngles);
      if (this.varType == typeof (Quaternion) && fromType == typeof (Vector3))
        return (Action<object>) (o => this.value = (object) Quaternion.Euler((Vector3) o));
      if (fromType == typeof (Vector3) && this.varType == typeof (Vector2))
        return (Action<object>) (o => this.value = (object) (Vector2) (Vector3) o);
      return fromType == typeof (Vector2) && this.varType == typeof (Vector3) ? (Action<object>) (o => this.value = (object) (Vector3) (Vector2) o) : (Action<object>) null;
    }

    public override string ToString() => this.name;
  }
}

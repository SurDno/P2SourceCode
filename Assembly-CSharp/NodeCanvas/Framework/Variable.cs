using System;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;

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
      get => _name;
      set
      {
        if (!(_name != value))
          return;
        _name = value;
        Action<string> onNameChanged = this.onNameChanged;
        if (onNameChanged != null)
          onNameChanged(value);
      }
    }

    public string ID
    {
      get
      {
        if (string.IsNullOrEmpty(_id))
          _id = Guid.NewGuid().ToString();
        return _id;
      }
    }

    public object value
    {
      get => objectValue;
      set => objectValue = value;
    }

    public bool isProtected
    {
      get => _protected;
      set => _protected = value;
    }

    protected bool HasValueChangeEvent() => onValueChanged != null;

    protected void OnValueChanged(string name, object value) => onValueChanged(name, value);

    protected abstract object objectValue { get; set; }

    public abstract Type varType { get; }

    public abstract bool hasBinding { get; }

    public abstract string propertyPath { get; set; }

    public abstract void BindProperty(MemberInfo prop, GameObject target = null);

    public abstract void UnBindProperty();

    public abstract void InitializePropertyBinding(GameObject go, bool callSetter = false);

    public bool CanConvertTo(Type toType) => GetGetConverter(toType) != null;

    public Func<object> GetGetConverter(Type toType)
    {
      if (toType.RTIsAssignableFrom(varType))
        return (Func<object>) (() => value);
      if (typeof (IConvertible).RTIsAssignableFrom(toType) && typeof (IConvertible).RTIsAssignableFrom(varType))
        return (Func<object>) (() =>
        {
          try
          {
            return Convert.ChangeType(value, toType);
          }
          catch
          {
            return !toType.RTIsAbstract() ? Activator.CreateInstance(toType) : null;
          }
        });
      if (toType == typeof (Transform) && varType == typeof (GameObject))
        return (Func<object>) (() => value != null ? (object) (value as GameObject).transform : (object) (Transform) null);
      if (toType == typeof (GameObject) && typeof (Component).RTIsAssignableFrom(varType))
        return (Func<object>) (() => value != null ? (object) (value as Component).gameObject : (object) (GameObject) null);
      if (toType == typeof (Vector3) && varType == typeof (GameObject))
        return (Func<object>) (() => (object) (value != null ? (value as GameObject).transform.position : Vector3.zero));
      if (toType == typeof (Vector3) && varType == typeof (Transform))
        return (Func<object>) (() => (object) (value != null ? (value as Transform).position : Vector3.zero));
      if (toType == typeof (Vector3) && varType == typeof (Quaternion))
        return (Func<object>) (() => (object) ((Quaternion) value).eulerAngles);
      if (toType == typeof (Quaternion) && varType == typeof (Vector3))
        return (Func<object>) (() => (object) Quaternion.Euler((Vector3) value));
      if (toType == typeof (Vector3) && varType == typeof (Vector2))
        return (Func<object>) (() => (object) (Vector3) (Vector2) value);
      return toType == typeof (Vector2) && varType == typeof (Vector3) ? (Func<object>) (() => (object) (Vector2) (Vector3) value) : null;
    }

    public bool CanConvertFrom(Type fromType) => GetSetConverter(fromType) != null;

    public Action<object> GetSetConverter(Type fromType)
    {
      if (varType.RTIsAssignableFrom(fromType))
        return o => value = o;
      if (typeof (IConvertible).RTIsAssignableFrom(varType) && typeof (IConvertible).RTIsAssignableFrom(fromType))
        return o =>
        {
          try
          {
            value = Convert.ChangeType(o, varType);
          }
          catch
          {
            value = !varType.RTIsAbstract() ? Activator.CreateInstance(varType) : null;
          }
        };
      if (varType == typeof (Transform) && fromType == typeof (GameObject))
        return o => value = o != null ? (object) (o as GameObject).transform : (object) (Transform) null;
      if (varType == typeof (GameObject) && typeof (Component).RTIsAssignableFrom(fromType))
        return o => value = o != null ? (object) (o as Component).gameObject : (object) (GameObject) null;
      if (varType == typeof (GameObject) && fromType == typeof (Vector3))
        return o =>
        {
          if (value == null)
            return;
          (value as GameObject).transform.position = (Vector3) o;
        };
      if (varType == typeof (Transform) && fromType == typeof (Vector3))
        return o =>
        {
          if (value == null)
            return;
          (value as Transform).position = (Vector3) o;
        };
      if (varType == typeof (Vector3) && fromType == typeof (Quaternion))
        return o => value = (object) ((Quaternion) o).eulerAngles;
      if (varType == typeof (Quaternion) && fromType == typeof (Vector3))
        return o => value = (object) Quaternion.Euler((Vector3) o);
      if (fromType == typeof (Vector3) && varType == typeof (Vector2))
        return o => value = (object) (Vector2) (Vector3) o;
      return fromType == typeof (Vector2) && varType == typeof (Vector3) ? o => value = (object) (Vector3) (Vector2) o : null;
    }

    public override string ToString() => name;
  }
}

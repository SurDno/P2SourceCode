using System;
using System.Linq.Expressions;
using Engine.Source.Settings;

namespace Engine.Source.Debugs
{
  public struct BoolPlayerProperty(string name, bool defaultValue = false) {
    private bool defaultValue = defaultValue;
    private bool value = PlayerSettings.Instance.GetBool(name, defaultValue);

    public static BoolPlayerProperty Create<T>(
      Expression<Func<T>> propertyLambda,
      bool defaultValue = false)
    {
      if (!(propertyLambda.Body is MemberExpression body))
        throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
      return new BoolPlayerProperty(typeof (BoolPlayerProperty).Name + "_" + body.Member.DeclaringType.Name + "_" + body.Member.Name, defaultValue);
    }

    public bool Value
    {
      get => value;
      set
      {
        this.value = value;
        PlayerSettings.Instance.SetBool(name, value);
        PlayerSettings.Instance.Save();
      }
    }

    public static implicit operator bool(BoolPlayerProperty property) => property.value;

    public override string ToString() => value.ToString();

    public override int GetHashCode() => value.GetHashCode();
  }
}

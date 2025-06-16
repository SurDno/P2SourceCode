// Decompiled with JetBrains decompiler
// Type: Engine.Source.Debugs.BoolPlayerProperty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Settings;
using System;
using System.Linq.Expressions;

#nullable disable
namespace Engine.Source.Debugs
{
  public struct BoolPlayerProperty
  {
    private string name;
    private bool defaultValue;
    private bool value;

    public BoolPlayerProperty(string name, bool defaultValue = false)
    {
      this.name = name;
      this.defaultValue = defaultValue;
      this.value = PlayerSettings.Instance.GetBool(name, defaultValue);
    }

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
      get => this.value;
      set
      {
        this.value = value;
        PlayerSettings.Instance.SetBool(this.name, value);
        PlayerSettings.Instance.Save();
      }
    }

    public static implicit operator bool(BoolPlayerProperty property) => property.value;

    public override string ToString() => this.value.ToString();

    public override int GetHashCode() => this.value.GetHashCode();
  }
}

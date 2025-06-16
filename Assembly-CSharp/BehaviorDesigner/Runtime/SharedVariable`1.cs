// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.SharedVariable`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public abstract class SharedVariable<T> : SharedVariable
  {
    [DataReadProxy(MemberEnum.None, Name = "Value")]
    [DataWriteProxy(MemberEnum.None, Name = "Value")]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected T mValue;

    public T Value
    {
      get => this.mValue;
      set => this.mValue = value;
    }

    public override object GetValue() => (object) this.Value;

    public override void SetValue(object value) => this.mValue = (T) value;

    public override string ToString()
    {
      return (object) this.Value == null ? "(null)" : this.Value.ToString();
    }
  }
}

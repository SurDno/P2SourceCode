using Engine.Common.Generator;
using UnityEngine;

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

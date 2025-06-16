using Engine.Common.Generator;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
  public abstract class SharedVariable
  {
    [DataReadProxy(Name = "IsShared")]
    [DataWriteProxy(Name = "IsShared")]
    [CopyableProxy]
    [SerializeField]
    protected bool mIsShared;
    [DataReadProxy(Name = "Name")]
    [DataWriteProxy(Name = "Name")]
    [CopyableProxy()]
    [SerializeField]
    protected string mName;

    public bool IsShared
    {
      get => mIsShared;
      set => mIsShared = value;
    }

    public string Name
    {
      get => mName;
      set => mName = value;
    }

    public bool IsNone => mIsShared && string.IsNullOrEmpty(mName);

    public abstract object GetValue();

    public abstract void SetValue(object value);
  }
}

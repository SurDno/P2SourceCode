// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.SharedVariable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public abstract class SharedVariable
  {
    [DataReadProxy(MemberEnum.None, Name = "IsShared")]
    [DataWriteProxy(MemberEnum.None, Name = "IsShared")]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected bool mIsShared = false;
    [DataReadProxy(MemberEnum.None, Name = "Name")]
    [DataWriteProxy(MemberEnum.None, Name = "Name")]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected string mName;

    public bool IsShared
    {
      get => this.mIsShared;
      set => this.mIsShared = value;
    }

    public string Name
    {
      get => this.mName;
      set => this.mName = value;
    }

    public bool IsNone => this.mIsShared && string.IsNullOrEmpty(this.mName);

    public abstract object GetValue();

    public abstract void SetValue(object value);
  }
}

// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Composite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  public abstract class Composite : ParentTask
  {
    [Tooltip("Specifies the type of conditional abort. More information is located at http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=89.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected AbortType abortType = AbortType.None;

    public AbortType AbortType => this.abortType;
  }
}

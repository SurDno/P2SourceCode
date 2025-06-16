// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.BehaviorReference
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("Behavior Reference allows you to run another behavior tree within the current behavior tree.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=53")]
  [TaskIcon("BehaviorTreeReferenceIcon.png")]
  public abstract class BehaviorReference : Action
  {
    [Tooltip("External behavior array that this task should reference")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public ExternalBehaviorTree[] externalBehaviors;
    [Tooltip("Any variables that should be set for the specific tree")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedNamedVariable[] variables;
    [HideInInspector]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public bool collapsed;

    public ExternalBehaviorTree[] GetExternalBehaviors() => this.externalBehaviors;

    public override void OnReset() => this.externalBehaviors = (ExternalBehaviorTree[]) null;
  }
}

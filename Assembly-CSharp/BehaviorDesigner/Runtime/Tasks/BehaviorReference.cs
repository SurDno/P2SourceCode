using Engine.Common.Generator;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("Behavior Reference allows you to run another behavior tree within the current behavior tree.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=53")]
  [TaskIcon("BehaviorTreeReferenceIcon.png")]
  public abstract class BehaviorReference : Action
  {
    [Tooltip("External behavior array that this task should reference")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public ExternalBehaviorTree[] externalBehaviors;
    [Tooltip("Any variables that should be set for the specific tree")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedNamedVariable[] variables;
    [HideInInspector]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public bool collapsed;

    public ExternalBehaviorTree[] GetExternalBehaviors() => externalBehaviors;

    public override void OnReset() => externalBehaviors = null;
  }
}

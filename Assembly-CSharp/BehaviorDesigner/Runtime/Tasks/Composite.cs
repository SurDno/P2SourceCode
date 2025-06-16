using Engine.Common.Generator;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  public abstract class Composite : ParentTask
  {
    [Tooltip("Specifies the type of conditional abort. More information is located at http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=89.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    protected AbortType abortType = AbortType.None;

    public AbortType AbortType => abortType;
  }
}

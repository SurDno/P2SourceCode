using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Engine.Common.Generator;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Find closest infected in list and write to Result")]
  [TaskCategory("Pathologic")]
  public abstract class FindClosestInListBase : Conditional
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransformList InputList;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Result;

    protected abstract bool Filter(GameObject gameObject);

    public override void OnAwake()
    {
      if (this.InputList.Value != null)
        return;
      Debug.LogWarningFormat("{0}: empty InputList", (object) this.gameObject.name);
    }

    public override TaskStatus OnUpdate()
    {
      if (this.InputList.Value == null)
        return TaskStatus.Failure;
      float num = 1E+20f;
      int index1 = -1;
      for (int index2 = 0; index2 < this.InputList.Value.Count; ++index2)
      {
        float sqrMagnitude = (this.gameObject.transform.position - this.InputList.Value[index2].position).sqrMagnitude;
        if ((double) sqrMagnitude < (double) num && this.Filter(this.InputList.Value[index2].gameObject))
        {
          index1 = index2;
          num = sqrMagnitude;
        }
      }
      if (index1 < 0)
        return TaskStatus.Failure;
      this.Result.Value = this.InputList.Value[index1];
      return TaskStatus.Success;
    }
  }
}

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Engine.Common.Generator;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals;

[TaskDescription("Find closest infected in list and write to Result")]
[TaskCategory("Pathologic")]
public abstract class FindClosestInListBase : Conditional {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedTransformList InputList;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedTransform Result;

	protected abstract bool Filter(GameObject gameObject);

	public override void OnAwake() {
		if (InputList.Value != null)
			return;
		Debug.LogWarningFormat("{0}: empty InputList", gameObject.name);
	}

	public override TaskStatus OnUpdate() {
		if (InputList.Value == null)
			return TaskStatus.Failure;
		var num = 1E+20f;
		var index1 = -1;
		for (var index2 = 0; index2 < InputList.Value.Count; ++index2) {
			var sqrMagnitude = (gameObject.transform.position - InputList.Value[index2].position).sqrMagnitude;
			if (sqrMagnitude < (double)num && Filter(InputList.Value[index2].gameObject)) {
				index1 = index2;
				num = sqrMagnitude;
			}
		}

		if (index1 < 0)
			return TaskStatus.Failure;
		Result.Value = InputList.Value[index1];
		return TaskStatus.Success;
	}
}
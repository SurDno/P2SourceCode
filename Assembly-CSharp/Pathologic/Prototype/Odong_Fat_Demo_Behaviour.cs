using UnityEngine;

namespace Pathologic.Prototype;

public class Odong_Fat_Demo_Behaviour : StateMachineBehaviour {
	public override void OnStateEnter(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex) {
		if (Random.value < 0.5)
			animator.SetInteger("Next", 0);
		else {
			var num = Random.Range(1, 8);
			if (num == 4) {
				num = Random.Range(1, 8);
				if (num == 4)
					num = 5;
			}

			animator.SetInteger("Next", num);
		}
	}
}
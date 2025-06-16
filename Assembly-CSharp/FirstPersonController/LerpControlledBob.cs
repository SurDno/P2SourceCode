using System;
using System.Collections;
using UnityEngine;

namespace FirstPersonController;

[Serializable]
public class LerpControlledBob {
	public float BobAmount;
	public float BobDuration;
	private float m_Offset;

	public float Offset() {
		return m_Offset;
	}

	public IEnumerator DoBobCycle() {
		var t = 0.0f;
		while (t < (double)BobDuration) {
			m_Offset = Mathf.Lerp(0.0f, BobAmount, t / BobDuration);
			t += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}

		t = 0.0f;
		while (t < (double)BobDuration) {
			m_Offset = Mathf.Lerp(BobAmount, 0.0f, t / BobDuration);
			t += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}

		m_Offset = 0.0f;
	}
}
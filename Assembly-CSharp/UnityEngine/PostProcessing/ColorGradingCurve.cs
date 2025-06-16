using System;

namespace UnityEngine.PostProcessing;

[Serializable]
public sealed class ColorGradingCurve {
	public AnimationCurve curve;
	[SerializeField] private bool m_Loop;
	[SerializeField] private float m_ZeroValue;
	[SerializeField] private float m_Range;
	private AnimationCurve m_InternalLoopingCurve;

	public ColorGradingCurve(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds) {
		this.curve = curve;
		m_ZeroValue = zeroValue;
		m_Loop = loop;
		m_Range = bounds.magnitude;
	}

	public void Cache() {
		if (!m_Loop)
			return;
		var length = curve.length;
		if (length < 2)
			return;
		if (m_InternalLoopingCurve == null)
			m_InternalLoopingCurve = new AnimationCurve();
		var key1 = curve[length - 1];
		key1.time -= m_Range;
		var key2 = curve[0];
		key2.time += m_Range;
		m_InternalLoopingCurve.keys = curve.keys;
		m_InternalLoopingCurve.AddKey(key1);
		m_InternalLoopingCurve.AddKey(key2);
	}

	public float Evaluate(float t) {
		if (curve.length == 0)
			return m_ZeroValue;
		return !m_Loop || curve.length == 1 ? curve.Evaluate(t) : m_InternalLoopingCurve.Evaluate(t);
	}
}
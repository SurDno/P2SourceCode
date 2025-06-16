using System;
using UnityEngine;

namespace FirstPersonController;

[Serializable]
public class CurveControlledBob {
	public AnimationCurve Bobcurve = new(new Keyframe(0.0f, 0.0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0.0f),
		new Keyframe(1.5f, -1f), new Keyframe(2f, 0.0f));

	public float HorizontalBobRange = 0.33f;
	private float m_BobBaseInterval;
	private float m_CyclePositionX;
	private float m_CyclePositionY;
	private Vector3 m_OriginalCameraPosition;
	private float m_Time;
	public float VerticalBobRange = 0.33f;
	public float VerticaltoHorizontalRatio = 1f;

	public void Setup(Camera camera, float bobBaseInterval) {
		m_BobBaseInterval = bobBaseInterval;
		m_OriginalCameraPosition = camera.transform.localPosition;
		m_Time = Bobcurve[Bobcurve.length - 1].time;
	}

	public Vector3 DoHeadBob(float speed) {
		var x = m_OriginalCameraPosition.x + Bobcurve.Evaluate(m_CyclePositionX) * HorizontalBobRange;
		var y = m_OriginalCameraPosition.y + Bobcurve.Evaluate(m_CyclePositionY) * VerticalBobRange;
		m_CyclePositionX += speed * Time.deltaTime / m_BobBaseInterval;
		m_CyclePositionY += speed * Time.deltaTime / m_BobBaseInterval * VerticaltoHorizontalRatio;
		if (m_CyclePositionX > (double)m_Time)
			m_CyclePositionX -= m_Time;
		if (m_CyclePositionY > (double)m_Time)
			m_CyclePositionY -= m_Time;
		return new Vector3(x, y, 0.0f);
	}
}
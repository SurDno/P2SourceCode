using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public sealed class ColorGradingCurve(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds) {
    public AnimationCurve curve = curve;
    [SerializeField]
    private bool m_Loop = loop;
    [SerializeField]
    private float m_ZeroValue = zeroValue;
    [SerializeField]
    private float m_Range = bounds.magnitude;
    private AnimationCurve m_InternalLoopingCurve;

    public void Cache()
    {
      if (!m_Loop)
        return;
      int length = curve.length;
      if (length < 2)
        return;
      if (m_InternalLoopingCurve == null)
        m_InternalLoopingCurve = new AnimationCurve();
      Keyframe key1 = curve[length - 1];
      key1.time -= m_Range;
      Keyframe key2 = curve[0];
      key2.time += m_Range;
      m_InternalLoopingCurve.keys = curve.keys;
      m_InternalLoopingCurve.AddKey(key1);
      m_InternalLoopingCurve.AddKey(key2);
    }

    public float Evaluate(float t)
    {
      if (curve.length == 0)
        return m_ZeroValue;
      return !m_Loop || curve.length == 1 ? curve.Evaluate(t) : m_InternalLoopingCurve.Evaluate(t);
    }
  }
}

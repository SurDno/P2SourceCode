using System;
using System.Collections;

namespace FirstPersonController
{
  [Serializable]
  public class LerpControlledBob
  {
    public float BobAmount;
    public float BobDuration;
    private float m_Offset;

    public float Offset() => m_Offset;

    public IEnumerator DoBobCycle()
    {
      float t = 0.0f;
      while (t < (double) BobDuration)
      {
        m_Offset = Mathf.Lerp(0.0f, BobAmount, t / BobDuration);
        t += Time.deltaTime;
        yield return (object) new WaitForFixedUpdate();
      }
      t = 0.0f;
      while (t < (double) BobDuration)
      {
        m_Offset = Mathf.Lerp(BobAmount, 0.0f, t / BobDuration);
        t += Time.deltaTime;
        yield return (object) new WaitForFixedUpdate();
      }
      m_Offset = 0.0f;
    }
  }
}

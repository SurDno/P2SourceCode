using System;
using System.Collections;
using UnityEngine;

namespace FirstPersonController
{
  [Serializable]
  public class LerpControlledBob
  {
    public float BobAmount;
    public float BobDuration;
    private float m_Offset;

    public float Offset() => this.m_Offset;

    public IEnumerator DoBobCycle()
    {
      float t = 0.0f;
      while ((double) t < (double) this.BobDuration)
      {
        this.m_Offset = Mathf.Lerp(0.0f, this.BobAmount, t / this.BobDuration);
        t += Time.deltaTime;
        yield return (object) new WaitForFixedUpdate();
      }
      t = 0.0f;
      while ((double) t < (double) this.BobDuration)
      {
        this.m_Offset = Mathf.Lerp(this.BobAmount, 0.0f, t / this.BobDuration);
        t += Time.deltaTime;
        yield return (object) new WaitForFixedUpdate();
      }
      this.m_Offset = 0.0f;
    }
  }
}

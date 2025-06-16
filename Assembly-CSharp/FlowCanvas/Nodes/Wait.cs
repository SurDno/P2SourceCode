using System.Collections;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class Wait : LatentActionNode<float>
  {
    public float timeLeft { get; private set; }

    public override IEnumerator Invoke(float time)
    {
      timeLeft = time;
      while (timeLeft > 0.0)
      {
        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(timeLeft, 0.0f);
        yield return null;
      }
    }
  }
}

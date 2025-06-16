using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class Wait : LatentActionNode<float>
  {
    public float timeLeft { get; private set; }

    public override IEnumerator Invoke(float time)
    {
      this.timeLeft = time;
      while ((double) this.timeLeft > 0.0)
      {
        this.timeLeft -= Time.deltaTime;
        this.timeLeft = Mathf.Max(this.timeLeft, 0.0f);
        yield return (object) null;
      }
    }
  }
}

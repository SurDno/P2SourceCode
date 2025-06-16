// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.Wait
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

#nullable disable
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

// Decompiled with JetBrains decompiler
// Type: FirstPersonController.LerpControlledBob
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

#nullable disable
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

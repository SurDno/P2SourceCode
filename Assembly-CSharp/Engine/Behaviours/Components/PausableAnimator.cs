// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Components.PausableAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Components
{
  public class PausableAnimator : MonoBehaviour
  {
    private Animator animator;

    private void OnPauseEvent()
    {
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        return;
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        this.animator.SetFloat("Mecanim.Speed", 0.0f);
      else
        this.animator.SetFloat("Mecanim.Speed", 1f);
    }

    private void OnEnable()
    {
      this.animator = this.GetComponent<Animator>();
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        return;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += new Action(this.OnPauseEvent);
      this.OnPauseEvent();
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        return;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= new Action(this.OnPauseEvent);
    }
  }
}

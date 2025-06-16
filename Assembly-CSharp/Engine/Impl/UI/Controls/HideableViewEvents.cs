// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HideableViewEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class HideableViewEvents : HideableView
  {
    [SerializeField]
    private EventView trueEvent;
    [SerializeField]
    private EventView falseEvent;
    private Coroutine coroutine = (Coroutine) null;

    protected override void ApplyVisibility()
    {
      this.Cancel();
      if (!this.gameObject.activeInHierarchy)
        return;
      this.coroutine = this.StartCoroutine(this.InvokeEvent(this.Visible));
    }

    private void Cancel()
    {
      if (this.coroutine == null)
        return;
      this.StopCoroutine(this.coroutine);
    }

    private IEnumerator InvokeEvent(bool value)
    {
      yield return (object) new WaitForEndOfFrame();
      if (value)
        this.trueEvent?.Invoke();
      else
        this.falseEvent?.Invoke();
    }

    public override void SkipAnimation() => this.Cancel();
  }
}

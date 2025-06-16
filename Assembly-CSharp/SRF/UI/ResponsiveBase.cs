// Decompiled with JetBrains decompiler
// Type: SRF.UI.ResponsiveBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace SRF.UI
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (RectTransform))]
  public abstract class ResponsiveBase : SRMonoBehaviour
  {
    private bool _queueRefresh;

    protected RectTransform RectTransform => (RectTransform) this.CachedTransform;

    protected void OnEnable() => this._queueRefresh = true;

    protected void OnRectTransformDimensionsChange() => this._queueRefresh = true;

    protected void Update()
    {
      if (!this._queueRefresh)
        return;
      this.Refresh();
      this._queueRefresh = false;
    }

    protected abstract void Refresh();

    [ContextMenu("Refresh")]
    private void DoRefresh() => this.Refresh();
  }
}

using UnityEngine;

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

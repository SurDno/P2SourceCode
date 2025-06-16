namespace SRF.UI
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (RectTransform))]
  public abstract class ResponsiveBase : SRMonoBehaviour
  {
    private bool _queueRefresh;

    protected RectTransform RectTransform => (RectTransform) CachedTransform;

    protected void OnEnable() => _queueRefresh = true;

    protected void OnRectTransformDimensionsChange() => _queueRefresh = true;

    protected void Update()
    {
      if (!_queueRefresh)
        return;
      Refresh();
      _queueRefresh = false;
    }

    protected abstract void Refresh();

    [ContextMenu("Refresh")]
    private void DoRefresh() => Refresh();
  }
}

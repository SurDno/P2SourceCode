using SRF;

namespace SRDebugger.UI.Controls
{
  [RequireComponent(typeof (RectTransform))]
  public class ProfilerGraphAxisLabel : SRMonoBehaviour
  {
    private float _prevFrameTime;
    private float? _queuedFrameTime;
    private float _yPosition;
    public Text Text;

    private void Update()
    {
      if (!_queuedFrameTime.HasValue)
        return;
      SetValueInternal(_queuedFrameTime.Value);
      _queuedFrameTime = new float?();
    }

    public void SetValue(float frameTime, float yPosition)
    {
      if (_prevFrameTime == (double) frameTime && _yPosition == (double) yPosition)
        return;
      _queuedFrameTime = frameTime;
      _yPosition = yPosition;
    }

    private void SetValueInternal(float frameTime)
    {
      _prevFrameTime = frameTime;
      Text.text = "{0}ms ({1}FPS)".Fmt((object) Mathf.FloorToInt(frameTime * 1000f), (object) Mathf.RoundToInt(1f / frameTime));
      RectTransform cachedTransform = (RectTransform) CachedTransform;
      cachedTransform.anchoredPosition = new Vector2((float) ((double) cachedTransform.rect.width * 0.5 + 10.0), _yPosition);
    }
  }
}

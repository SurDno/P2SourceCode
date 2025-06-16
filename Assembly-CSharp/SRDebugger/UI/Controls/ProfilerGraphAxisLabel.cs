using SRF;
using UnityEngine;
using UnityEngine.UI;

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
      if (!this._queuedFrameTime.HasValue)
        return;
      this.SetValueInternal(this._queuedFrameTime.Value);
      this._queuedFrameTime = new float?();
    }

    public void SetValue(float frameTime, float yPosition)
    {
      if ((double) this._prevFrameTime == (double) frameTime && (double) this._yPosition == (double) yPosition)
        return;
      this._queuedFrameTime = new float?(frameTime);
      this._yPosition = yPosition;
    }

    private void SetValueInternal(float frameTime)
    {
      this._prevFrameTime = frameTime;
      this.Text.text = "{0}ms ({1}FPS)".Fmt((object) Mathf.FloorToInt(frameTime * 1000f), (object) Mathf.RoundToInt(1f / frameTime));
      RectTransform cachedTransform = (RectTransform) this.CachedTransform;
      cachedTransform.anchoredPosition = new Vector2((float) ((double) cachedTransform.rect.width * 0.5 + 10.0), this._yPosition);
    }
  }
}

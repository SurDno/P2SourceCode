using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRDebugger.UI.Controls
{
  public class MultiTapButton : Button
  {
    private float _lastTap;
    private int _tapCount;
    public int RequiredTapCount = 3;
    public float ResetTime = 0.5f;

    public override void OnPointerClick(PointerEventData eventData)
    {
      if ((double) Time.unscaledTime - (double) this._lastTap > (double) this.ResetTime)
        this._tapCount = 0;
      this._lastTap = Time.unscaledTime;
      ++this._tapCount;
      if (this._tapCount != this.RequiredTapCount)
        return;
      base.OnPointerClick(eventData);
      this._tapCount = 0;
    }
  }
}

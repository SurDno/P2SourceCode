// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Controls.ProfilerGraphAxisLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRF;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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

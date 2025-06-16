// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Other.ScrollRectPatch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace SRDebugger.UI.Other
{
  [RequireComponent(typeof (ScrollRect))]
  [ExecuteInEditMode]
  public class ScrollRectPatch : MonoBehaviour
  {
    public RectTransform Content;
    public Mask ReplaceMask;
    public RectTransform Viewport;

    private void Awake()
    {
      ScrollRect component = this.GetComponent<ScrollRect>();
      component.content = this.Content;
      component.viewport = this.Viewport;
      if (!((Object) this.ReplaceMask != (Object) null))
        return;
      GameObject gameObject = this.ReplaceMask.gameObject;
      Object.Destroy((Object) gameObject.GetComponent<Graphic>());
      Object.Destroy((Object) gameObject.GetComponent<CanvasRenderer>());
      Object.Destroy((Object) this.ReplaceMask);
      gameObject.AddComponent<RectMask2D>();
    }
  }
}

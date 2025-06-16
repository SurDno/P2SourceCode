// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Other.HandleManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRF;
using UnityEngine;

#nullable disable
namespace SRDebugger.UI.Other
{
  public class HandleManager : SRMonoBehaviour
  {
    private bool _hasSet;
    public GameObject BottomHandle;
    public GameObject BottomLeftHandle;
    public GameObject BottomRightHandle;
    public PinAlignment DefaultAlignment;
    public GameObject LeftHandle;
    public GameObject RightHandle;
    public GameObject TopHandle;
    public GameObject TopLeftHandle;
    public GameObject TopRightHandle;

    private void Start()
    {
      if (this._hasSet)
        return;
      this.SetAlignment(this.DefaultAlignment);
    }

    public void SetAlignment(PinAlignment alignment)
    {
      this._hasSet = true;
      switch (alignment)
      {
        case PinAlignment.TopLeft:
        case PinAlignment.TopRight:
          this.SetActive(this.BottomHandle, true);
          this.SetActive(this.TopHandle, false);
          this.SetActive(this.TopLeftHandle, false);
          this.SetActive(this.TopRightHandle, false);
          break;
        case PinAlignment.BottomLeft:
        case PinAlignment.BottomRight:
          this.SetActive(this.BottomHandle, false);
          this.SetActive(this.TopHandle, true);
          this.SetActive(this.BottomLeftHandle, false);
          this.SetActive(this.BottomRightHandle, false);
          break;
      }
      switch (alignment)
      {
        case PinAlignment.TopLeft:
        case PinAlignment.BottomLeft:
          this.SetActive(this.LeftHandle, false);
          this.SetActive(this.RightHandle, true);
          this.SetActive(this.TopLeftHandle, false);
          this.SetActive(this.BottomLeftHandle, false);
          break;
        case PinAlignment.TopRight:
        case PinAlignment.BottomRight:
          this.SetActive(this.LeftHandle, true);
          this.SetActive(this.RightHandle, false);
          this.SetActive(this.TopRightHandle, false);
          this.SetActive(this.BottomRightHandle, false);
          break;
      }
      switch (alignment)
      {
        case PinAlignment.TopLeft:
          this.SetActive(this.BottomLeftHandle, false);
          this.SetActive(this.BottomRightHandle, true);
          break;
        case PinAlignment.TopRight:
          this.SetActive(this.BottomLeftHandle, true);
          this.SetActive(this.BottomRightHandle, false);
          break;
        case PinAlignment.BottomLeft:
          this.SetActive(this.TopLeftHandle, false);
          this.SetActive(this.TopRightHandle, true);
          break;
        case PinAlignment.BottomRight:
          this.SetActive(this.TopLeftHandle, true);
          this.SetActive(this.TopRightHandle, false);
          break;
      }
    }

    private void SetActive(GameObject obj, bool active)
    {
      if ((Object) obj == (Object) null)
        return;
      obj.SetActive(active);
    }
  }
}

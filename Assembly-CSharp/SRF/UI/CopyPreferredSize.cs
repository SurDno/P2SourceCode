// Decompiled with JetBrains decompiler
// Type: SRF.UI.CopyPreferredSize
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace SRF.UI
{
  [RequireComponent(typeof (RectTransform))]
  [ExecuteInEditMode]
  [AddComponentMenu("SRF/UI/Copy Preferred Size")]
  public class CopyPreferredSize : LayoutElement
  {
    public RectTransform CopySource;
    public float PaddingHeight;
    public float PaddingWidth;

    public override float preferredWidth
    {
      get
      {
        return (Object) this.CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetPreferredWidth(this.CopySource) + this.PaddingWidth;
      }
    }

    public override float preferredHeight
    {
      get
      {
        return (Object) this.CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetPreferredHeight(this.CopySource) + this.PaddingHeight;
      }
    }

    public override int layoutPriority => 2;
  }
}

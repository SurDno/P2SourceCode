// Decompiled with JetBrains decompiler
// Type: SRF.UI.FlashGraphic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/Flash Graphic")]
  [ExecuteInEditMode]
  public class FlashGraphic : 
    UIBehaviour,
    IPointerDownHandler,
    IEventSystemHandler,
    IPointerUpHandler
  {
    public float DecayTime = 0.15f;
    public Color DefaultColor = new Color(1f, 1f, 1f, 0.0f);
    public Color FlashColor = Color.white;
    public Graphic Target;

    public void OnPointerDown(PointerEventData eventData)
    {
      this.Target.CrossFadeColor(this.FlashColor, 0.0f, true, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      this.Target.CrossFadeColor(this.DefaultColor, this.DecayTime, true, true);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.Target.CrossFadeColor(this.DefaultColor, 0.0f, true, true);
    }

    protected void Update()
    {
    }

    public void Flash()
    {
      this.Target.CrossFadeColor(this.FlashColor, 0.0f, true, true);
      this.Target.CrossFadeColor(this.DefaultColor, this.DecayTime, true, true);
    }
  }
}

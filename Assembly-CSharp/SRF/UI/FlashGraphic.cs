using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
      Target.CrossFadeColor(FlashColor, 0.0f, true, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      Target.CrossFadeColor(DefaultColor, DecayTime, true, true);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      Target.CrossFadeColor(DefaultColor, 0.0f, true, true);
    }

    protected void Update()
    {
    }

    public void Flash()
    {
      Target.CrossFadeColor(FlashColor, 0.0f, true, true);
      Target.CrossFadeColor(DefaultColor, DecayTime, true, true);
    }
  }
}

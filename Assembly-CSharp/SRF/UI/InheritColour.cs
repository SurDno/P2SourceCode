using UnityEngine;
using UnityEngine.UI;

namespace SRF.UI
{
  [RequireComponent(typeof (Graphic))]
  [ExecuteInEditMode]
  [AddComponentMenu("SRF/UI/Inherit Colour")]
  public class InheritColour : SRMonoBehaviour
  {
    private Graphic _graphic;
    public Graphic From;

    private Graphic Graphic
    {
      get
      {
        if ((Object) this._graphic == (Object) null)
          this._graphic = this.GetComponent<Graphic>();
        return this._graphic;
      }
    }

    private void Refresh()
    {
      if ((Object) this.From == (Object) null)
        return;
      this.Graphic.color = this.From.canvasRenderer.GetColor();
    }

    private void Update() => this.Refresh();

    private void Start() => this.Refresh();
  }
}

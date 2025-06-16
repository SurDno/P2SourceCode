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
        if ((Object) _graphic == (Object) null)
          _graphic = this.GetComponent<Graphic>();
        return _graphic;
      }
    }

    private void Refresh()
    {
      if ((Object) From == (Object) null)
        return;
      Graphic.color = From.canvasRenderer.GetColor();
    }

    private void Update() => Refresh();

    private void Start() => Refresh();
  }
}

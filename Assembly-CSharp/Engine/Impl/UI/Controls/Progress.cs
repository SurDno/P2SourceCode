namespace Engine.Impl.UI.Controls
{
  public class Progress : UIControl
  {
    [SerializeField]
    [FormerlySerializedAs("_Image")]
    private GameObject image = (GameObject) null;

    public Material Material { get; protected set; }

    public float Value
    {
      get => (Object) Material == (Object) null ? 0.0f : Material.GetFloat("_Progress");
      set
      {
        if ((Object) Material == (Object) null)
          return;
        Material.SetFloat("_Progress", value);
      }
    }

    public bool IsVisible
    {
      get => this.gameObject.activeSelf;
      set => this.gameObject.SetActive(value);
    }

    protected override void Awake()
    {
      base.Awake();
      if ((Object) image == (Object) null)
        return;
      RawImage component = image.GetComponent<RawImage>();
      if ((Object) component == (Object) null)
        return;
      Material = Object.Instantiate<Material>(component.material);
      component.material = Material;
    }
  }
}
